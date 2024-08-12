using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.School.API.Application.Commands;
using Sxb.School.API.Application.Models;
using Sxb.School.API.Application.Queries.SchoolViewOrder;
using Sxb.School.API.Infrastructures.Services;
using Sxb.School.API.Infrastructures.Services.Models;
using Sxb.School.API.Models;
using Sxb.School.Domain.AggregateModels.ViewOrder;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sxb.School.API.Controllers
{
    using Dapper;
    using Sxb.School.API.Filters;

    [Route("api/[controller]")]
    [ApiController]
    public class ViewPayController : ControllerBase
    {
        IMediator _mediator;
        IEasyRedisClient _easyRedisClient;
        ILogger<ViewPayController> _logger;
        public ViewPayController(IMediator mediator
            , IEasyRedisClient easyRedisClient, ILogger<ViewPayController> logger)
        {
            _mediator = mediator;
            _easyRedisClient = easyRedisClient;
            _logger = logger;
        }


        /// <summary>
        /// 检查是否已解锁
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="schoolViewOrderQueries"></param>
        /// <returns></returns>
        [HttpGet("CheckDeLock")]
        public async Task<ResponseResult> CheckDeLock(Guid eid
             , [FromServices] ISchoolViewOrderQueries schoolViewOrderQueries
            )
        {
            if (!await schoolViewOrderQueries.IsALevelSchool(eid))
            {
                return ResponseResult.Success(true);
            }
            if (!HttpContext.User.Identity.IsAuthenticated)
                return ResponseResult.Success(false);
            var userInfo = HttpContext.GetUserInfo();
            Guid userId = userInfo.UserId;
            if (await schoolViewOrderQueries.ExistsPermissionAsync(userId, eid))
                return ResponseResult.Success(true);
            else
                return ResponseResult.Success(false);



        }



        [HttpGet("getqrcode")]
        public async Task<ResponseResult> CreateOrderQRCode([FromQuery] CreateViewSchoolQRCodeCommand cmd
            , [FromServices] IConfiguration configuration)
        {
            var userInfo = HttpContext.GetUserInfo();
            cmd.UserId = userInfo?.UserId;
            string checkCode = Guid.NewGuid().ToString();
            cmd.ScanCallBackUrl = $"{configuration["RootPath"]}/api/ViewPay/RCVWPCB_VS?checkCode={checkCode}";
            var res = await _mediator.Send(cmd);
            await _easyRedisClient.AddStringAsync($"CreateOrderQRCode:{res.order.Id}", checkCode, res.order.ExpireTime - DateTime.Now);
            return ResponseResult.Success(new
            {
                orderId = res.order.Id,
                qrCodeUrl = res.qrcodeUrl
            });
        }

        [HttpGet("getQRCodeState")]
        public async Task<ResponseResult> GetQRCodeState(
            [FromQuery] Guid orderId,
            [FromServices] ISchoolViewOrderQueries schoolViewOrderQueries,
            [FromServices] IUserCenterService userCenterService)
        {
            var stateSummary = await schoolViewOrderQueries.GetViewOrderStateAsync(orderId);
            if (stateSummary == null)
                return ResponseResult.Success(new { state = ViewOrderState.WaitPay });
            if (stateSummary.State == ViewOrderState.PaySuccess)
            {
                //获取登录码
                string code = await userCenterService.GetLoginCodeAsync(stateSummary.UserId);
                return ResponseResult.Success(new
                {
                    state = stateSummary.State,
                    code = code
                });

            }
            return ResponseResult.Success(new
            {
                state = stateSummary.State
            });
        }


        /// <summary>
        /// 接收微信服务号关注/扫码回调
        /// </summary>
        /// <param name="checkCode"></param>
        /// <param name="callBackData"></param>
        /// <param name="payGatewayService"></param>
        /// <param name="weChatGatewayService"></param>
        /// <param name="schoolViewOrderQueries"></param>
        /// <param name="configuration"></param>
        /// <param name="wxWorkService"></param>
        /// <returns></returns>
        [HttpPost("RCVWPCB_VS")]
        public async Task<ResponseResult> ReceiveWPCallBackOfViewSchool([FromQuery] string checkCode
            , [FromBody] WPScanCallBackData callBackData
            , [FromServices] IPayGatewayService payGatewayService
            , [FromServices] IWeChatGatewayService weChatGatewayService
            , [FromServices] ISchoolViewOrderQueries schoolViewOrderQueries
            , [FromServices] IConfiguration configuration
            , [FromServices] IWxWorkService wxWorkService
            )
        {

            ViewSchoolScanCallBackAttachData attachData = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewSchoolScanCallBackAttachData>(callBackData.Attach);
            //设置订单的用户ID
            SetUserForViewSchoolOrderCommand setUserForOrderCmd = new SetUserForViewSchoolOrderCommand() { OrderId = attachData.OrderId, UserId = callBackData.UserId };
            var orderDetail = await _mediator.Send(setUserForOrderCmd);
            bool isScanForSelf = callBackData.UserId == orderDetail.UserId;
            //var orderDetail = await schoolViewOrderQueries.GetOrderDetailAsync(attachData.OrderId);
            try
            {
                using (var con = new SqlConnection(configuration["ConnectionString:Master"]))
                {
                    string sql1 = @"SELECT createTime,LASTSUBSCRIBETIME subscibeTime FROM iSchoolUser.dbo.OPENID_WEIXIN WHERE OpenId=@openId";
                    var openIdInfo = await con.QueryFirstOrDefaultAsync(sql1, new { openId = callBackData.OpenId });


                    string sql2 = @"INSERT  INTO iSchoolWeChat.dbo.ViewSchoolSubscribers VALUES(@id,@eid,@userId,@isFree,@createTime,@isNewSubscribe)";
                    await con.ExecuteAsync(sql2,
                        new
                        {
                            id = Guid.NewGuid(),
                            eid = orderDetail.GetViewSchoolGoodsInfo().Id,
                            userId = callBackData.UserId,
                            isFree = attachData.Mode == 1,
                            createTime = DateTime.Now,
                            isNewSubscribe = openIdInfo.subscibeTime == null ? false : ((TimeSpan)(openIdInfo.subscibeTime - openIdInfo.createTime)).TotalMinutes < 1
                        });
                }

            }
            catch { }

#if DEBUG
#else
            string checkCodeCache = await _easyRedisClient.GetStringAsync($"CreateOrderQRCode:{attachData.OrderId}");
            if (checkCode.Equals(checkCodeCache) == false) return ResponseResult.Failed("验证码错误。");
#endif

            var schoolInfo = await schoolViewOrderQueries.GetSchoolInfo(orderDetail.GetViewSchoolGoodsInfo().Id);
            var userInfo = await schoolViewOrderQueries.GetUserInfo(orderDetail.UserId.Value);
            //支付中心回调地址链接 
            string payCallBackUrl = $"{configuration["RootPath"]}/api/ViewPay/RCVPAYCB_VS";
            string wxWorkAddCustomerCallBackUrl = $"{configuration["RootPath"]}/api/ViewPay/RCVWXWORKCB_VS";

            if (await schoolViewOrderQueries.ExistsPermissionAsync(callBackData.UserId, schoolInfo.EID))
            {
                await weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"您已解锁过 {schoolInfo.Name}，在电脑网页完成登录即可查看全部信息");
                return ResponseResult.Success("OK");
            }

            if (attachData.Mode == 1)
            {
                if (await schoolViewOrderQueries.ExistsFreeOrder(callBackData.UserId))
                {
                    //更改订单价格
                    await _mediator.Send(new UpdateViewSchoolOrderCommand() { Amount = Configs.ViewSchoolUnitPrice, OrderId = orderDetail.Id });
                    //生成支付订单
                    var (orderNo, orderId) = await payGatewayService.PayByH5(new AddPayOrderRequest(callBackData.UserId.ToString()
                          , attachData.OrderId.ToString(), orderDetail.Num, Configs.ViewSchoolUnitPrice, Configs.ViewSchoolUnitPrice
                          , new AddPayOrderRequest.Orderbyproduct[] { new AddPayOrderRequest.Orderbyproduct() {
                          productId = orderDetail.GetViewSchoolGoodsInfo().Id.ToString()
                          , amount =  Configs.ViewSchoolUnitPrice
                          , buyNum = 1
                          , orderId = attachData.OrderId.ToString()
                          , remark = $"{schoolInfo.Name}{Configs.ViewSchoolUnitPrice}元解锁"
                          , advanceOrderId = attachData.OrderId.ToString()
                          , price = 1
                          , orderDetailId = attachData.OrderId.ToString()
                          , productType = 4
                          , status = (int)ViewOrderState.WaitPay} }
                          , callBackData.OpenId, attachData.OrderId.ToString(), $"{schoolInfo.Name}{Configs.ViewSchoolUnitPrice}元解锁"
                          , payCallBackUrl
                          , orderDetail.ExpireTime));
                    //发送tips
                    if (isScanForSelf)
                    {
                        //为自己扫
                        await weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"”首次免费解锁“服务每位用户仅有一次机会，您已使用过。如仍需查看该校全部信息，点击下方链接，支付{ Configs.ViewSchoolUnitPrice}元解锁{schoolInfo.Name}全部信息。");

                    }
                    else
                    {
                        //为别人扫
                        await weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"”首次免费解锁“服务每位用户仅有一次机会，您已使用过。如仍需查看该校全部信息，点击下方链接，为账号“{userInfo.NickName}”支付{ Configs.ViewSchoolUnitPrice}元解锁{schoolInfo.Name}全部信息。");

                    }

                    //发送支付链接
                    string payUrl = $"{configuration["ExternalInterface:MAddress"]}pay/wechatPay?orderId={orderId}&orderNo={orderNo}";
                    await weChatGatewayService.SendNewsMsg(callBackData.OpenId, "点击支付", "", payUrl, "https://cos.sxkid.com/images/logo_share_v4.png");

                }
                else
                {

                    //生成客服二维码
                    var getAddCustomerQrCodeRequest = new GetAddCustomerQrCodeRequest()
                    {
                        openId = callBackData.OpenId,
                        notifyUrl = wxWorkAddCustomerCallBackUrl
                    };
                    getAddCustomerQrCodeRequest.SetAttachData(new WxWorkAddCustomerCallBackData() { OrderId = attachData.OrderId });
                    var customerQrCodeRet = await wxWorkService.GetAddCustomerQrCode(getAddCustomerQrCodeRequest);
                    //发送tips
                    if (isScanForSelf)
                    {
                        //为自己扫
                        await weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"长按识别下方二维码，添加上学帮工作人员微信，解锁{schoolInfo.Name}全部信息");
                    }
                    else
                    {
                        //为别人扫
                        await weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"您正在为账号“{userInfo.NickName}”使用“首次免费解锁”服务。长按识别下方二维码，添加上学帮工作人员微信，解锁{schoolInfo.Name}全部信息");
                    }

                    //发送客服二维码
                    await weChatGatewayService.SendImgMsg(callBackData.OpenId, await DownLoadFileHelper.DownloadStream(customerQrCodeRet.qrcodeUrl));
                }

            }
            else if (attachData.Mode == 2)
            {


                var (orderNo, orderId) = await payGatewayService.PayByH5(
                    new AddPayOrderRequest(callBackData.UserId.ToString()
                      , attachData.OrderId.ToString(), orderDetail.Num, orderDetail.Amount, orderDetail.Amount
                      , new AddPayOrderRequest.Orderbyproduct[] { new AddPayOrderRequest.Orderbyproduct() {
                          productId = orderDetail.GetViewSchoolGoodsInfo().Id.ToString()
                          , amount = orderDetail.Amount
                          , buyNum = 1
                          , orderId = attachData.OrderId.ToString()
                          , remark =  $"{schoolInfo.Name}{Configs.ViewSchoolUnitPrice}元解锁"
                          , advanceOrderId = attachData.OrderId.ToString()
                          , price =1
                          , orderDetailId = attachData.OrderId.ToString()
                          , productType = 4
                          , status = (int)ViewOrderState.WaitPay} }
                      , callBackData.OpenId, attachData.OrderId.ToString(), $"{schoolInfo.Name}{Configs.ViewSchoolUnitPrice}元解锁"
                      , payCallBackUrl
                      , orderDetail.ExpireTime)
                    );

                //发送tips
                await weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"您正在为账号“{userInfo.NickName}”支付{Configs.ViewSchoolUnitPrice}元解锁{schoolInfo.Name}全部信息，点击下方链接进行付款");
                //发送支付链接
                string payUrl = $"{configuration["ExternalInterface:MAddress"]}pay/wechatPay?orderId={orderId}&orderNo={orderNo}";
                await weChatGatewayService.SendNewsMsg(callBackData.OpenId, "点击支付", "", payUrl, "https://cos.sxkid.com/images/logo_share_v4.png");

            }

            return ResponseResult.Success("OK");
        }


        /// <summary>
        /// 接收支付中心支付结果回调
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("RCVPAYCB_VS")]
        [PayCheckSign]
        public async Task<ResponseResult> ReceivePayCallBackOfViewSchool([FromBody] PayCallBackData data)
        {
            _logger.LogInformation($"接收到支付平台结果回调：{Newtonsoft.Json.JsonConvert.SerializeObject(data)}");

            if (data.PayStatus == PayCallBackData.PayStatusEnum.Success)
            {
                //支付成功
                ViewSchoolOrderPaySuccessCommand cmd = new ViewSchoolOrderPaySuccessCommand()
                {
                    OrderId = data.OrderId,
                    PayWay = ViewOrderPayWay.WeChatPay
                };
                await _mediator.Send(cmd);
            }
            else if (data.PayStatus == PayCallBackData.PayStatusEnum.Fail)
            {
                //支付失败
                ViewSchoolOrderPayFailCommand cmd = new ViewSchoolOrderPayFailCommand()
                {
                    OrderId = data.OrderId,
                    PayWay = ViewOrderPayWay.WeChatPay
                };
                await _mediator.Send(cmd);
            }


            return ResponseResult.Success("OK");
        }

        /// <summary>
        /// 接收加客服成功回调
        /// </summary>
        /// <returns></returns>

        [HttpPost("RCVWXWORKCB_VS")]
        [InnerRequestValidate]
        public async Task<ResponseResult> ReceiveWXWorkCallBackOfViewSchool(
            [FromBody] WxWorkAddCustomerCallBackData callBackData,
            [FromServices] ISchoolViewOrderQueries schoolViewOrderQueries,
            [FromServices] IConfiguration configuration)
        {
            _logger.LogInformation($"接收到WXWORK平台结果回调：{Newtonsoft.Json.JsonConvert.SerializeObject(callBackData)}");

            try
            {
                using (var con = new SqlConnection(configuration["ConnectionString:Master"]))
                {
                    var orderDetail = await schoolViewOrderQueries.GetOrderDetailAsync(callBackData.OrderId);
                    string sql = @"INSERT INTO iSchoolWeChat.dbo.AddWXCustomers VALUES(@id,@eid,@createTime)";
                    await con.ExecuteAsync(sql, new { id = Guid.NewGuid(), eid = orderDetail.GetViewSchoolGoodsInfo().Id, createTime = DateTime.Now });
                }
            }
            catch { }

            ViewSchoolOrderPaySuccessCommand cmd = new ViewSchoolOrderPaySuccessCommand()
            {
                OrderId = callBackData.OrderId,
                PayWay = ViewOrderPayWay.SubscribeWPFree,
            };
            await _mediator.Send(cmd);

            return ResponseResult.Success("OK");
        }

        [HttpGet("testauth")]
        public ResponseResult TestAuth()
        {
            if (HttpContext.Request.Cookies.TryGetValue("iSchoolAuth", out string iSchoolAuth))
            {
                _logger.LogInformation($"iSchoolAuth：{iSchoolAuth}");
            }
            else
            {
                _logger.LogInformation($"iSchoolAuth：不存在");
            }

            _logger.LogInformation($"登录状态：{HttpContext.User.Identity.IsAuthenticated}");
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return ResponseResult.Success($"iSchoolAuth:{iSchoolAuth}");
            }
            else {
                return ResponseResult.Failed($"iSchoolAuth:{iSchoolAuth}");
            }
    
        }

    }
}
