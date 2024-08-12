using MediatR;
using Microsoft.AspNetCore.Authorization;
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
using Sxb.School.API.Application.Queries.DgAyOrder;
using Sxb.School.API.Application.Queries.DegreeAnalyze;
using Sxb.School.API.Infrastructures.Services;
using Sxb.School.API.Infrastructures.Services.Models;
using Sxb.School.API.Models;
using Sxb.School.API.RequestContact.DegreeAnalyze;
using Sxb.School.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sxb.School.API.Application.Queries.UserInfo;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using Sxb.School.API.Filters;
using Sxb.School.API.Application.Queries.DgAyUserQaPaper;
using System.Data.SqlClient;
using Dapper;
using Sxb.School.API.Application.Queries.DgAyAddCustomerUser;

namespace Sxb.School.API.Controllers
{
    /// <summary>
    /// 2022学位分析器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DegreeAnalyzeController : ControllerBase
    {
        ILogger<DegreeAnalyzeController> _logger;
        readonly IEasyRedisClient _easyRedisClient;
        readonly IMediator _mediator;
        readonly IDegreeAnalyzeQueries _degreeAnalyzeQueries;
        readonly IConfiguration _configuration;

        public DegreeAnalyzeController(IEasyRedisClient easyRedisClient
            , IDegreeAnalyzeQueries degreeAnalyzeQueries
            , IMediator mediator
            , ILogger<DegreeAnalyzeController> logger
            ,IConfiguration configuration)
        {
            _easyRedisClient = easyRedisClient;
            this._mediator = mediator;
            this._degreeAnalyzeQueries = degreeAnalyzeQueries;
            _logger = logger;
            _configuration = configuration;


        }




        /// <summary>
        /// 解锁分析结果前置信息
        /// </summary>
        /// <param name="paperId">分析结果ID</param>
        /// <param name="termtyp">终端类型 1->h5 2->pc 3->小程序</param>
        /// <param name="dgAyUserQaPaperQueries"></param>
        /// <param name="dgAyOrderQueries"></param>
        /// <returns></returns>
        public async Task<ResponseResult> DeLockPreInfo(
          [FromQuery] Guid paperId, [FromQuery] byte termtyp
            , [FromServices] IDgAyUserQaPaperQueries dgAyUserQaPaperQueries
            , [FromServices] IDgAyOrderQueries dgAyOrderQueries
            , [FromServices] IUserInfoQueries  userInfoQueries
            , [FromServices] IDgAyAddCustomerUserQueries dgAyAddCustomerUserQueries
            , [FromServices] IWxWorkService   wxWorkService
            , [FromServices] IWeChatGatewayService weChatGatewayService
            )
        {
            var wxWorkAddCustomerCallBackUrl = $"{_configuration["RootPath"]}{Url.Action("RCVWXWORKCB_VS")}";
            var userInfo = HttpContext.GetUserInfo();
            var now = DateTime.Now;
            Guid? orderId = null;
            if (termtyp == 1 && (now > Configs.DgAyFreeSTime && now < Configs.DgAyFreeETime))
            {
                var openId = await userInfoQueries.GetOpenId(userInfo.UserId);
                //已关注服务号并且已加客服
                if (string.IsNullOrEmpty(openId) == false && (await dgAyAddCustomerUserQueries.GetStatus(userInfo.UserId)))
                {

                    //todo:
                    //    0. 创建订单
                    var productInfos = new List<DgAyOrderProductInfo>();
                    decimal unitPrice = Configs.DgAyResultUnitPrice;
                    decimal originUnitPrice = Configs.DgAyResultUnitPrice;
                    productInfos.Add(new DgAyOrderProductInfo
                    {
                        Number = 1,
                        ProductId = paperId,
                        productType = DgAyProductType.DgAyResult,
                        ProductName = "学位分析结果",
                        ProductDesc = "学位分析结果",
                        OriginUnitPrice = originUnitPrice,
                        UnitPrice = unitPrice,
                    });
                    CreateDgAyOrderCommand createcmd = new CreateDgAyOrderCommand() { UserId = userInfo?.UserId, Termtyp = termtyp, ProductInfos = productInfos };
                    var order = await _mediator.Send(createcmd);
                    orderId = order.Id;
                    //    1.1 已加，直接进行解锁操作（完成订单）
                    PayDgAyOrderCommand paycmd = new PayDgAyOrderCommand()
                    {
                        OrderId = order.Id,
                        PayWay = DgAyOrderPayWay.SubscribeWPFree,
                    };
                    await _mediator.Send(paycmd);
                    //此处延迟一秒，以下操作数据是从读库读出，避免数据同步延迟
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    var paper = await dgAyUserQaPaperQueries.GetDgAyUserQaPaper(paperId);
                    //发送tips
                    string tplId = _configuration["WXTplIds:DgAyOrderDeLock"];
                    await weChatGatewayService.SendTplMsg(openId, tplId, $"{_configuration["ExternalInterface:MAddress"]}analysis/resultlist/{paper.Id}", new List<TplDataFiled>() {
                        new TplDataFiled(){  Filed = "first", Value = "\n解锁成功！"},
                        new TplDataFiled(){  Filed = "keyword1", Value = $"{paper.Title}"},
                        new TplDataFiled(){  Filed = "keyword2", Value = "已解锁"},
                        new TplDataFiled(){  Filed = "remark", Value = "点击查看已解锁的分析结果"}});


                }
            }
            var alevelIds = await dgAyUserQaPaperQueries.GetALevelEIdsWithUnOpenPermission(paperId);
            bool existsFreeOrder = await dgAyOrderQueries.ExistsFreeOrder(userInfo.UserId);
            decimal ZPrice = 0;
            if (termtyp == 2)
                //终端为PC时，算上学部解锁钱
                ZPrice = (Configs.ViewSchoolUnitPrice * alevelIds.Count * (decimal)0.2);
            return ResponseResult.Success(new
            {
                HasFreeChance = !existsFreeOrder,
                BasePrice = Configs.DgAyResultUnitPrice,
                XPrice = ZPrice + Configs.DgAyResultUnitPrice,
                YCount = alevelIds.Count,
                ZPrice = ZPrice,
                OrderId = orderId,
                FreeSTime = Configs.DgAyFreeSTime.ToUnixTimestampByMilliseconds(),
                FreeETime = Configs.DgAyFreeETime.ToUnixTimestampByMilliseconds(),
            }, "OK");
        }


        /// <summary>
        /// 获取订单二维码
        /// </summary>
        /// <param name="paperId">分析结果ID</param>
        /// <param name="mode">1 -> 免费 2->仅解锁结果 3->既解锁结果又解锁A级学校 4->限时免费</param>
        /// <param name="termtyp">终端类型 1->h5 2->pc 3->小程序</param>
        /// <param name="configuration"></param>
        /// <param name="weChatGatewayService"></param>
        /// <param name="dgAyUserQaPaperQueries"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("getqrcode")]
        public async Task<ResponseResult> GetOrderQRCode([FromQuery] Guid paperId
        , [FromQuery] int mode
        , [FromQuery] byte termtyp
        , [FromServices] IConfiguration configuration
        , [FromServices] IWeChatGatewayService weChatGatewayService
        , [FromServices] IDgAyUserQaPaperQueries dgAyUserQaPaperQueries)
        {
            var userInfo = HttpContext.GetUserInfo();

            //判断模式
            //3模式且终端为PC时需要查询结果中所包含的A级别学校，计算出费用 1 * num * 0.2
            var productInfos = new List<DgAyOrderProductInfo>();
            decimal unitPrice = Configs.DgAyResultUnitPrice;
            decimal originUnitPrice = Configs.DgAyResultUnitPrice;
            if (mode == 3 && termtyp == 2)
            {
                var alevelEIds = await dgAyUserQaPaperQueries.GetALevelEIdsWithUnOpenPermission(paperId);
                unitPrice = (Configs.ViewSchoolUnitPrice * alevelEIds.Count * (decimal)0.2) + Configs.DgAyResultUnitPrice;
                originUnitPrice = (Configs.ViewSchoolUnitPrice * alevelEIds.Count) + Configs.DgAyResultUnitPrice;
                foreach (var aLevelEId in alevelEIds)
                {
                    productInfos.Add(new DgAyOrderProductInfo
                    {
                        Number = 1,
                        ProductId = aLevelEId,
                        productType = DgAyProductType.ViewALevelSchoolPermission ,
                        ProductName = "A级学校开通查阅权限",
                        ProductDesc = "A级学校开通查阅权限",
                        OriginUnitPrice = Configs.ViewSchoolUnitPrice,
                        UnitPrice = 0,
                    });
                }
            }
            productInfos.Add(new DgAyOrderProductInfo
            {
                Number = 1,
                ProductId = paperId,
                productType = DgAyProductType.DgAyResult,
                ProductName = "学位分析结果",
                ProductDesc = "学位分析结果",
                OriginUnitPrice = originUnitPrice,
                UnitPrice = unitPrice,
            });

            CreateDgAyOrderCommand cmd = new CreateDgAyOrderCommand(){ UserId = userInfo?.UserId,Termtyp = termtyp,ProductInfos = productInfos };
            var order = await _mediator.Send(cmd);
            //生成二维码
            CreateDgAyOrderScanCallBackAttachData attachData = new CreateDgAyOrderScanCallBackAttachData()
            {
                OrderId = order.Id,
                Mode = mode,
                Termtyp = termtyp

            };
            string qrcodeImgUrl = await weChatGatewayService.GetSenceQRCode(new GetSenceQRCodeRequest()
            {
                app = 0,
                callBackUrl = $"{configuration["RootPath"]}{Url.Action("RCVWPCB_VS")}",
                expireSecond = (int)(order.ExpireTime - DateTime.Now).TotalSeconds,
                attach = Newtonsoft.Json.JsonConvert.SerializeObject(attachData),
                fw = "delock_DgAyRes"

            });

            return ResponseResult.Success(new
            {
                orderId = order.Id,
                qrCodeUrl = qrcodeImgUrl
            });
        }


        [HttpGet]
        [ActionName("getQRCodeState")]
        public async Task<ResponseResult> GetQRCodeState(
    [FromQuery] Guid orderId,
    [FromServices] IDgAyOrderQueries dgAyOrderQueries,
    [FromServices] IUserCenterService userCenterService)
        {
            var stateSummary = await dgAyOrderQueries.GetDgAyOrderStateAsync(orderId);
            if (stateSummary.State == Domain.AggregateModels.DgAyOrderAggregate.DgAyOrderState.PaySuccess)
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
        /// <param name="dgAyOrderQueries"></param>
        /// <param name="configuration"></param>
        /// <param name="wxWorkService"></param>
        /// <param name="userInfoQueries"></param>
        /// <param name="dgAyUserQaPaperQueries"></param>
        /// <param name="easyRedisClient"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RCVWPCB_VS")]
        public async Task<ResponseResult> ReceiveWPCallBackOfViewSchool([FromQuery] string checkCode
            , [FromBody] WPScanCallBackData callBackData
            , [FromServices] IPayGatewayService payGatewayService
            , [FromServices] IWeChatGatewayService weChatGatewayService
            , [FromServices] IDgAyOrderQueries dgAyOrderQueries
            , [FromServices] IConfiguration configuration
            , [FromServices] IWxWorkService wxWorkService
            , [FromServices] IUserInfoQueries userInfoQueries
            , [FromServices] IDgAyUserQaPaperQueries dgAyUserQaPaperQueries
            , [FromServices] IEasyRedisClient easyRedisClient
            , [FromServices] IDgAyAddCustomerUserQueries dgAyAddCustomerUserQueries
            )
        {


            CreateDgAyOrderScanCallBackAttachData attachData = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateDgAyOrderScanCallBackAttachData>(callBackData.Attach);
            var orderDetail = await dgAyOrderQueries.GetOrderDetailAsync(attachData.OrderId);
            await easyRedisClient.AddAsync(RedisCacheKeys.WXCB_DHAY_Order(orderDetail.Id), callBackData, orderDetail.ExpireTime);
            //设置订单的用户ID
            SetDgAyOrderUserCommand setUserForOrderCmd = new SetDgAyOrderUserCommand() { OrderId = attachData.OrderId, UserId = callBackData.UserId };
            var updateUserState = await _mediator.Send(setUserForOrderCmd);
            var userInfo = await userInfoQueries.GetUserInfo(callBackData.UserId);
            //订单拥有者的Id
            Guid? orderUserId = updateUserState ? callBackData.UserId : orderDetail.UserId;
            var orderUserInfo = await userInfoQueries.GetUserInfo(orderUserId.GetValueOrDefault());
            var paperProduct = orderDetail.ProductInfos.FirstOrDefault(s => s.Type == DgAyProductType.DgAyResult);
            var paper = await dgAyUserQaPaperQueries.GetDgAyUserQaPaper(paperProduct.Id);

            //支付中心回调地址链接 
            string payCallBackUrl = $"{configuration["RootPath"]}{Url.Action("RCVPAYCB_VS")}";
            var wxWorkAddCustomerCallBackUrl = $"{_configuration["RootPath"]}{Url.Action("RCVWXWORKCB_VS")}";

            if (attachData.Mode == 1)
            {
                //免费模式
                if (!await dgAyOrderQueries.ExistsFreeOrder(callBackData.UserId))
                {
                    //生成客服二维码
                    var getAddCustomerQrCodeRequest = new GetAddCustomerQrCodeRequest()
                    {
                        openId = callBackData.OpenId,
                        notifyUrl = wxWorkAddCustomerCallBackUrl,
                        scene = "DegreeAnalyze"
                    };
                    getAddCustomerQrCodeRequest.SetAttachData(new WxWorkAddCustomerCallBackData() { OrderId = attachData.OrderId });
                    var customerQrCodeRet = await wxWorkService.GetAddCustomerQrCode(getAddCustomerQrCodeRequest);
                    //发送tips
                    if (orderUserId == callBackData.UserId)
                    {
                        //支付人订单拥有者是同一个人
                        await weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"长按识别下方二维码，添加上学帮工作人员微信，解锁{paper.AnalyzedTime?.ToString("yyyy年MM月dd日")}学位分析结果");
                    }
                    else
                    {
                        //支付人订单拥有者不是同一个人
                        await weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"您正在为账号“{orderUserInfo.NickName}”使用“首次免费解锁”服务。长按识别下方二维码，添加上学帮工作人员微信，解锁{paper.AnalyzedTime?.ToString("yyyy年MM月dd日")}学位分析结果");
                    }

                    //发送客服二维码
                    await weChatGatewayService.SendImgMsg(callBackData.OpenId, await DownLoadFileHelper.DownloadStream(customerQrCodeRet.qrcodeUrl));
                }
                else
                {
                    //付费模式
                    //跳转到支付流程
                    var (orderNo, orderId) = await payGatewayService.PayByH5(
                        new AddPayOrderRequest(callBackData.UserId.ToString()
                          , attachData.OrderId.ToString(), orderDetail.Num, orderDetail.Amount, orderDetail.Amount
                          , new AddPayOrderRequest.Orderbyproduct[] { new AddPayOrderRequest.Orderbyproduct() {
                          productId = paperProduct.Id.ToString()
                          , amount = orderDetail.Amount
                          , buyNum = paperProduct.Number
                          , orderId = attachData.OrderId.ToString()
                          , remark = paperProduct.Name  ?? ""
                          , advanceOrderId = attachData.OrderId.ToString()
                          , price = paperProduct.UnitPrice
                          , orderDetailId = attachData.OrderId.ToString()
                          , productType = 4
                          , status = (int)DgAyOrderState.WaitPay} }
                          , callBackData.OpenId, attachData.OrderId.ToString(), paperProduct.Name ?? ""
                          , payCallBackUrl
                          , orderDetail.ExpireTime)
                        );

                    //发送tips
                    await weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"”首次免费解锁“服务每位用户仅有一次机会，您已使用过。如仍需继续解锁，点击下方链接，支付{orderDetail.Amount.ToString("0.##")}元查看。");
                    //发送支付链接
                    string payUrl = $"{configuration["ExternalInterface:MAddress"]}pay/wechatPay?orderId={orderId}&orderNo={orderNo}";
                    await weChatGatewayService.SendNewsMsg(callBackData.OpenId, "点击支付", "", payUrl, "https://cos.sxkid.com/images/logo_share_v4.png");

                }

            }
            else if (attachData.Mode == 4)
            {
                DateTime now = DateTime.Now;
                //限时免费模式
                //todo:
                //    1.判断是否在期限内
                if ((now > Configs.DgAyFreeSTime && now < Configs.DgAyFreeETime))
                {
                    //    1.1 在期限内，判断是否已加了微信客服
                    if (await dgAyAddCustomerUserQueries.GetStatus(callBackData.UserId))
                    {
                        //    1.1.2 已加客服，订单成功。
                        PayDgAyOrderCommand paycmd = new PayDgAyOrderCommand()
                        {
                            OrderId = attachData.OrderId,
                            PayWay = DgAyOrderPayWay.SubscribeWPFree,
                        };
                        await _mediator.Send(paycmd);
                        //发送tips
                        string tplId = _configuration["WXTplIds:DgAyOrderDeLock"];
                        await weChatGatewayService.SendTplMsg(callBackData.OpenId, tplId, $"{_configuration["ExternalInterface:MAddress"]}analysis/resultlist/{paper.Id}", new List<TplDataFiled>() {
                        new TplDataFiled(){  Filed = "first", Value = "\n解锁成功！"},
                        new TplDataFiled(){  Filed = "keyword1", Value = $"{paper.Title}"},
                        new TplDataFiled(){  Filed = "keyword2", Value = "已解锁"},
                        new TplDataFiled(){  Filed = "remark", Value = "点击查看已解锁的分析结果"}});
                    }
                    else {
                        //    1.1.1 未加客服，服务号发送加客服提示
                        //生成客服二维码
                        var getAddCustomerQrCodeRequest = new GetAddCustomerQrCodeRequest()
                        {
                            openId = callBackData.OpenId,
                            notifyUrl = wxWorkAddCustomerCallBackUrl,
                            scene = "DegreeAnalyze"
                        };
                        getAddCustomerQrCodeRequest.SetAttachData(new WxWorkAddCustomerCallBackData() { OrderId = attachData.OrderId });
                        var customerQrCodeRet = await wxWorkService.GetAddCustomerQrCode(getAddCustomerQrCodeRequest);
                        //支付人订单拥有者是同一个人
                        await weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"长按识别下方二维码，添加上学帮工作人员微信，解锁{paper.AnalyzedTime?.ToString("yyyy年MM月dd日")}学位分析结果");
                        //发送客服二维码
                        await weChatGatewayService.SendImgMsg(callBackData.OpenId, await DownLoadFileHelper.DownloadStream(customerQrCodeRet.qrcodeUrl));
                    }

                }

                }
            else
            {
                //付费模式
                //跳转到支付流程
                var (orderNo, orderId) = await payGatewayService.PayByH5(
                    new AddPayOrderRequest(callBackData.UserId.ToString()
                      , attachData.OrderId.ToString(), orderDetail.Num, orderDetail.Amount, orderDetail.Amount
                      , new AddPayOrderRequest.Orderbyproduct[] { new AddPayOrderRequest.Orderbyproduct() {
                          productId = paperProduct.Id.ToString()
                          , amount = orderDetail.Amount
                          , buyNum = paperProduct.Number 
                          , orderId = attachData.OrderId.ToString()
                          , remark = paperProduct.Name  ?? ""
                          , advanceOrderId = attachData.OrderId.ToString()
                          , price = paperProduct.UnitPrice
                          , orderDetailId = attachData.OrderId.ToString()
                          , productType = 4
                          , status = (int)DgAyOrderState.WaitPay} }
                      , callBackData.OpenId, attachData.OrderId.ToString(), paperProduct.Name ?? ""
                      , payCallBackUrl
                      , orderDetail.ExpireTime)
                    );

                //发送tips
                await weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"您正在为账号“{orderUserInfo.NickName}”支付{orderDetail.Amount.ToString("0.##")}元解锁{paper.AnalyzedTime?.ToString("yyyy年MM月dd日")}学位分析结果全部信息，点击下方链接进行付款");
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
        /// <param name="weChatGatewayService"></param>
        /// <param name="dgAyOrderQueries"></param>
        /// <param name="dgAyUserQaPaperQueries"></param>
        /// <param name="configuration"></param>
        /// <param name="easyRedisClient"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RCVPAYCB_VS")]
        #if DEBUG
        #else
        [PayCheckSign]
        #endif
        public async Task<ResponseResult> ReceivePayCallBackOfViewSchool([FromBody] PayCallBackData data
            , [FromServices] IWeChatGatewayService weChatGatewayService
            , [FromServices] IDgAyOrderQueries dgAyOrderQueries
            , [FromServices] IDgAyUserQaPaperQueries dgAyUserQaPaperQueries
            , [FromServices] IConfiguration configuration
            , [FromServices] IEasyRedisClient easyRedisClient)
        {
            _logger.LogInformation($"接收到支付平台结果回调：{Newtonsoft.Json.JsonConvert.SerializeObject(data)}");

            if (data.PayStatus == PayCallBackData.PayStatusEnum.Success)
            {
                //支付成功
                PayDgAyOrderCommand cmd = new PayDgAyOrderCommand()
                {
                    OrderId = data.OrderId,
                    PayWay = DgAyOrderPayWay.WeChatPay
                };
                await _mediator.Send(cmd);
                //此处延迟一秒，以下操作数据是从读库读出，避免数据同步延迟
                await Task.Delay(TimeSpan.FromSeconds(1));
                var orderDetail = await dgAyOrderQueries.GetOrderDetailAsync(data.OrderId);
                //订单拥有者的Id
                var paperProduct = orderDetail.ProductInfos.FirstOrDefault(s => s.Type == DgAyProductType.DgAyResult);
                var paper = await dgAyUserQaPaperQueries.GetDgAyUserQaPaper(paperProduct.Id);
                var callBackData = await easyRedisClient.GetAsync<WPScanCallBackData>(RedisCacheKeys.WXCB_DHAY_Order(orderDetail.Id));
                CreateDgAyOrderScanCallBackAttachData attachData = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateDgAyOrderScanCallBackAttachData>(callBackData.Attach);
                if (attachData.Termtyp == 1)
                {    //发送tips
                    string tplId = configuration["WXTplIds:DgAyOrderDeLock"];
                    await weChatGatewayService.SendTplMsg(callBackData.OpenId, tplId, $"{configuration["ExternalInterface:MAddress"]}analysis/resultlist/{paper.Id}", new List<TplDataFiled>() {
                    new TplDataFiled(){  Filed = "first", Value = "\n解锁成功！"},
                    new TplDataFiled(){  Filed = "keyword1", Value = $"{paper.Title}"},
                    new TplDataFiled(){  Filed = "keyword2", Value = "已解锁"},
                    new TplDataFiled(){  Filed = "remark", Value = "点击查看已解锁的分析结果"}});
                }
                await easyRedisClient.RemoveAsync(RedisCacheKeys.WXCB_DHAY_Order(orderDetail.Id));
            }
            else if (data.PayStatus == PayCallBackData.PayStatusEnum.Fail)
            {
                //支付失败
                SetDgAyOrderStatusCommand cmd = new SetDgAyOrderStatusCommand()
                {
                    OrderId = data.OrderId,
                    State = DgAyOrderState.PayFail
                };
                await _mediator.Send(cmd);
            }

            return ResponseResult.Success("OK");
        }

        /// <summary>
        /// 接收加客服成功回调
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [ActionName("RCVWXWORKCB_VS")]
        #if DEBUG
        #else
              [InnerRequestValidate]
        #endif
        public async Task<ResponseResult> ReceiveWXWorkCallBackOfViewSchool(
            [FromBody] WxWorkAddCustomerCallBackData callBackData
            , [FromServices] IWeChatGatewayService weChatGatewayService
            , [FromServices] IDgAyOrderQueries dgAyOrderQueries
            , [FromServices] IDgAyUserQaPaperQueries dgAyUserQaPaperQueries
            , [FromServices] IConfiguration configuration
            , [FromServices] IEasyRedisClient easyRedisClient)
        {
            _logger.LogInformation($"接收到WXWORK平台结果回调：{Newtonsoft.Json.JsonConvert.SerializeObject(callBackData)}");
            PayDgAyOrderCommand cmd = new PayDgAyOrderCommand()
            {
                OrderId = callBackData.OrderId,
                PayWay = DgAyOrderPayWay.SubscribeWPFree,
            };
            await _mediator.Send(cmd);

            //此处延迟一秒，以下操作数据是从读库读出，避免数据同步延迟
            await Task.Delay(TimeSpan.FromSeconds(1));
            var orderDetail = await dgAyOrderQueries.GetOrderDetailAsync(callBackData.OrderId);
            //订单拥有者的Id
            var paperProduct = orderDetail.ProductInfos.FirstOrDefault(s => s.Type == DgAyProductType.DgAyResult);
            var paper = await dgAyUserQaPaperQueries.GetDgAyUserQaPaper(paperProduct.Id);
            var wxcallBackData = await easyRedisClient.GetAsync<WPScanCallBackData>(RedisCacheKeys.WXCB_DHAY_Order(orderDetail.Id));
            try
            {
                //：记录加了微信客服的userId
                using (var con = new SqlConnection(configuration["ConnectionString:Master"]))
                {
                    string sql = @"IF NOT EXISTS(SELECT 1 FROM [iSchoolData].[dbo].[DgAyAddCustomerUser] WHERE USERID  = @UserId)
BEGIN
	INSERT INTO [iSchoolData].[dbo].[DgAyAddCustomerUser]  VALUES(@UserId)
END";
                    await con.ExecuteAsync(sql, new { userId = wxcallBackData.UserId });
                }
            }
            catch { }

            //发送tips
            CreateDgAyOrderScanCallBackAttachData createDgAyOrderScanCallBackAttachData = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateDgAyOrderScanCallBackAttachData>(wxcallBackData.Attach);
            if (createDgAyOrderScanCallBackAttachData.Termtyp == 1)
            {   //发送tips
                string tplId =  configuration["WXTplIds:DgAyOrderDeLock"];
                await weChatGatewayService.SendTplMsg(wxcallBackData.OpenId, tplId, $"{configuration["ExternalInterface:MAddress"]}analysis/resultlist/{paper.Id}", new List<TplDataFiled>() {
                    new TplDataFiled(){  Filed = "first", Value = "\n解锁成功！"},
                    new TplDataFiled(){  Filed = "keyword1", Value = $"{paper.Title}"},
                    new TplDataFiled(){  Filed = "keyword2", Value = "已解锁"},
                    new TplDataFiled(){  Filed = "remark", Value = "点击查看已解锁的分析结果"}});
            }
            await easyRedisClient.RemoveAsync(RedisCacheKeys.WXCB_DHAY_Order(orderDetail.Id));
            return ResponseResult.Success("OK");
        }




        /// <summary>
        /// 获取题目s
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResult<DgAyGetQuestionResponse>), 200)]
        public async Task<ResponseResult> GetQuestions()
        {
            try
            {
                var r = await _degreeAnalyzeQueries.GetQuestions(readCache: true, writeCache: true);
                return ResponseResult.Success(r);
            }
            catch (ResponseResultException ex)
            {
                return ResponseResult.Failed((ResponseCode)ex.Code, ex.Message, null);
            }
        }

        /// <summary>
        /// 题型type=5时,根据选择了的地区,获取具体地址
        /// </summary>
        /// <param name="area">地区编码</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResult<DgAyGetQuesAddressesResponse>), 200)]
        public async Task<ResponseResult> GetQuesAddresses(int area)
        {
            try
            {
                var r = await _degreeAnalyzeQueries.GetQuesAddresses(area);
                return ResponseResult.Success(r);
            }
            catch (ResponseResultException ex)
            {
                return ResponseResult.Failed((ResponseCode)ex.Code, ex.Message, null);
            }
        }

        /// <summary>
        /// (分页)题型type=5时,根据选择了的地区,获取具体地址
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResult<PagedList<string>>), 200)]
        public async Task<ResponseResult> GetQuesAddresses(DgAyFindAddressesQuery query)
        {
            try
            {
                var r = await _degreeAnalyzeQueries.GetQuesAddresses(query);
                return ResponseResult.Success(r);
            }
            catch (ResponseResultException ex)
            {
                return ResponseResult.Failed((ResponseCode)ex.Code, ex.Message, null);
            }
        }

        /// <summary>
        /// 提交问卷
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(APIResult<DgAySubmitQaCmdResult>), 200)]
        public async Task<ResponseResult> Submit(DgAySubmitQaCmd cmd)
        {
            var user = HttpContext.GetUserInfo();
            cmd.UserId = user.UserId;
            try
            {
                var r = await _mediator.Send(cmd);
                return ResponseResult.Success(r);
            }
            catch (ResponseResultException ex)
            {
                return ResponseResult.Failed((ResponseCode)ex.Code, ex.Message, null);   
            }            
        }

        /// <summary>
        /// 我的往期分析结果
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize">默认20条一页</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(APIResult<PagedList<DgAyMyQaResultListItem>>), 200)]
        public async Task<ResponseResult> MyQaResultList(int pageIndex, int pageSize = 20)
        {
            try
            {
                var user = HttpContext.GetUserInfo();
                if (user == null) throw new ResponseResultException("请登录", 401);
                var r = await _degreeAnalyzeQueries.GetMyQaResultList(user.UserId, pageIndex, pageSize);
                return ResponseResult.Success(r);
            }
            catch (ResponseResultException ex)
            {
                return ResponseResult.Failed((ResponseCode)ex.Code, ex.Message, null);
            }
        }

        /// <summary>
        /// 我的分析报告（含未解锁和已解锁）
        /// </summary>
        /// <param name="id">报告id</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(APIResult<DgAyQaResultVm>), 200)]
        public async Task<ResponseResult> MyQa(Guid id)
        {
            try
            {
                var user = HttpContext.GetUserInfo();
                var r = await _degreeAnalyzeQueries.GetQaResult(id, me: user?.UserId ?? Guid.Empty);
                return ResponseResult.Success(r);
            }
            catch (ResponseResultException ex)
            {
                return ResponseResult.Failed((ResponseCode)ex.Code, ex.Message, null);
            }
        }

        /// <summary>
        /// 我的答题情况(仅仅答题内容)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(APIResult<DgAyQaResultVm0>), 200)]
        public async Task<ResponseResult> MyQaCtn(Guid id)
        {
            try
            {
                var r = await _degreeAnalyzeQueries.GetQaCtn(id);
                return ResponseResult.Success(r);
            }
            catch (ResponseResultException ex)
            {
                return ResponseResult.Failed((ResponseCode)ex.Code, ex.Message, null);
            }
        }

        /// <summary>
        /// 我的分析报告是否已解锁
        /// </summary>
        /// <param name="id">报告id</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(APIResult<DgAyQaIsUnlockedVm>), 200)]
        public async Task<ResponseResult> IsUnlocked(Guid id)
        {
            var user = HttpContext.GetUserInfo();
            var r = await _degreeAnalyzeQueries.IsUnlocked(id, me: user?.UserId ?? Guid.Empty);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 统计文件下载
        /// </summary>
        /// <param name="token">sxbyyds</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Statistic(string token)
        {
            if (string.IsNullOrEmpty(token) || !token.Equals("sxbyyds"))
                return NotFound();
            

            return Ok();
        }

    }
}
