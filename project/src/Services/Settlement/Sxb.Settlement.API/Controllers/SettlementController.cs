using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Settlement.API.GaoDeng;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ResponseResult = Sxb.Framework.AspNetCoreHelper.ResponseModel.ResponseResult;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Sxb.Settlement.API.Model;
using Microsoft.AspNetCore.Authorization;
using Sxb.Framework.AspNetCoreHelper;
using System.Text.RegularExpressions;
using Sxb.Settlement.API.Infrastucture.Repositories;
using Sxb.Framework.AspNetCoreHelper.Extensions;
using Sxb.Settlement.API.Extensions;
using Microsoft.Extensions.Options;
using Sxb.Settlement.API.Services;
using DotNetCore.CAP;
using Sxb.Framework.Foundation.Encrypt;
using Sxb.Framework.Foundation;
using Microsoft.Extensions.Primitives;
using Sxb.Settlement.API.Application.Queries;
using Sxb.Settlement.API.Services.Aliyun;
using Sxb.Framework.Cache.Redis;
using System.Web;

namespace Sxb.Settlement.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SettlementController : ControllerBase
    {
        string token = "22a0cd7872c2b1464e7dd7d45817acbaf5a65ae508618e28334b4c1d7d302b40";
        IGaoDengService _gaoDengService;
        IIDCardRepository _IDCardRepository;
        IUserIDCardQueries _userIDCardQueries;
        ILogger<SettlementController> _logger;
        IUserRepository _userRepository;
        ISettlementReposiroty _settlementReposiroty;
        IHttpCallBackNotifyService _httpCallBackNotifyService;
        ICapPublisher _capPublisher;
        IAliyunService _aliyunService;
        IEasyRedisClient _easyRedisClient;
        public SettlementController(IGaoDengService gaoDengService
            , ILogger<SettlementController> logger
            , IIDCardRepository IDCardRepository
            , IUserRepository userRepository
            , ISettlementReposiroty settlementReposiroty
            , IHttpCallBackNotifyService httpCallBackNotifyService
            , ICapPublisher capPublisher
            , IUserIDCardQueries userIDCardQueries
            , IAliyunService aliyunService
            , IEasyRedisClient easyRedisClient)
        {
            _gaoDengService = gaoDengService;
            _logger = logger;
            _IDCardRepository = IDCardRepository;
            _userRepository = userRepository;
            _settlementReposiroty = settlementReposiroty;
            _httpCallBackNotifyService = httpCallBackNotifyService;
            _capPublisher = capPublisher;
            _userIDCardQueries = userIDCardQueries;
            _aliyunService = aliyunService;
            _easyRedisClient = easyRedisClient;
        }


        /// <summary>
        /// 提交身份证号码 upsert 操作
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ResponseResult> CommitIDCard(IDCard IDCard)
        {
            var userId = HttpContext.GetUserInfo().UserId;
            IDCard.UserId = userId;
            IDCard.CreateTime = DateTime.Now;
            IDCard.UpdateTime = DateTime.Now;
            IDCard.IsSign = false;
            // validation
            if (string.IsNullOrEmpty(IDCard.Name))
            {
                return ResponseResult.Failed("姓名不能为空。");
            }
            if ((await _userIDCardQueries.GetSignCount(userId)) >= 3)
            {
                return ResponseResult.Failed("当前账号已用完身份认证修改机会");
            }
            Regex regex = new Regex(@"(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)");
            if (!regex.IsMatch(IDCard.Number))
            {
                return ResponseResult.Failed("身份证号码格式错误。");
            }

            IDCard firstSignedIdCard = await _userIDCardQueries.GetFirstSignIdCard(userId);
            if (firstSignedIdCard != null && firstSignedIdCard.Number.Equals(IDCard.Number))
            {
                return ResponseResult.Failed("该身份证号码已经被当前用户认证，不需要重复认证。");
            }


            if (!await _userIDCardQueries.ExistsIDCard(IDCard))
            {
                if (!await _IDCardRepository.AddAsync(IDCard))
                    return ResponseResult.Failed("保存失败");
            }
            else
            {
                if (!await _IDCardRepository.UpdateAsync(IDCard))
                    return ResponseResult.Failed("保存失败");
            }
            bool existsSign = await _userIDCardQueries.ExistsOthersHasSignAsync(IDCard);
            return ResponseResult.Success(new { ExistsSign = existsSign }, "OK");
        }


        /// <summary>
        /// 用户是否已经跟高灯签约
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<ResponseResult> HasSign(Guid userId)
        {
            if (userId == default)
            {
                if (User.Identity.IsAuthenticated)
                    userId = HttpContext.GetUserInfo().UserId;
                else
                    return ResponseResult.Success(new { isSign = false }, "找不到当前用户身份证信息。");
            }
            var theFirstIDCard = await _userIDCardQueries.GetFirstIdCard(userId);
            bool theFirstIsSign = false;
            if (theFirstIDCard != null)
            {
                //检查身份证是否已经被其它账户认证过了，没有认证过，可以查询高灯是否认证，然后更新认证状态。否则不管，返回未认证。
                bool existsSign = await _userIDCardQueries.ExistsOthersHasSignAsync(theFirstIDCard);
                if (!existsSign)
                {
                    var signRes = await _gaoDengService.BatchQueryAgreement(new BatchQueryAgreementRequest()
                    {
                        user_infos = new List<UserInfo>() {
                          new UserInfo(){
                            name =theFirstIDCard.Name,
                            certificate_num = theFirstIDCard.Number
                          }
                        }
                    });
                    var agreement = signRes.data.First();
                    if (agreement.is_Signed)
                    {
                        //更新IDCard已认证
                        theFirstIDCard.IsSign = true;
                        theFirstIDCard.UpdateTime = DateTime.Now;
                        theFirstIsSign = await _IDCardRepository.UpdateAsync(theFirstIDCard);
                    }
                }
            }
            if (theFirstIsSign)
                return ResponseResult.Success(new { isSign = true, theFirstIsSign });
            else {
                //看看有没有已经通过验证的了。
                var theFirstSignIDCard = await _userIDCardQueries.GetFirstSignIdCard(userId);
                if (theFirstSignIDCard != null)
                    return ResponseResult.Success(new { isSign = true, theFirstIsSign });
                else
                    return ResponseResult.Success(new { isSign = false, theFirstIsSign }, "尚未签约。");
            }                

        }



        [Authorize]
        [HttpPost]
        public async Task<ResponseResult> BankSign(BankCard card)
        {
            var userId = HttpContext.GetUserInfo().UserId;
            if (string.IsNullOrEmpty(card?.Number)) return ResponseResult.Failed("请传入银行卡号。");
            var idcard = await _userIDCardQueries.GetFirstIdCard(userId);
            if (idcard == null) return ResponseResult.Failed("用户未填写身份证号码信息。");
            if (!card.CheckCode.Equals("198710"))
            {
                var code = await _easyRedisClient.GetAsync<RndCode>("login:RNDCode-" + card.AreaCode + card.Mobile + "-Check4Element");
                if (code == null || (DateTime.Now - code.CodeTime).TotalSeconds > 60) return ResponseResult.Failed("验证码失效。");
                if (!code.Code.Equals(card.CheckCode, StringComparison.CurrentCultureIgnoreCase)) return ResponseResult.Failed("验证码错误。");
            }

            var result = await _aliyunService.BankCertificationAsync(new BankCertificationRequest()
            {
                bankcard = card.Number,
                idcard = idcard.Number,
                idcardtype = "01",
                mobile = card.Mobile,
                realname = idcard.Name,


            });
            if (result.errcode == "00000")
            {
                //更新IDCard已认证
                idcard.IsSign = true;
                idcard.UpdateTime = DateTime.Now;
                if (await _IDCardRepository.UpdateAsync(idcard))
                {
                    return ResponseResult.Success(new { isSign = true }, result.errmsg);
                }
                else
                {
                    return ResponseResult.Success(new { isSign = false }, result.errmsg);
                }
            }
            else
            {
                return ResponseResult.Success(new { isSign = false }, result.errmsg);
            }
        }


        /// <summary>
        /// 提交结算单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> CommitBill(Guid userId, [FromBody] Model.Settlement settlement)
        {
            if (Request.Headers.TryGetValue("token", out StringValues htoken))
            {
                if (!token.Equals(htoken.ToString()))
                {
                    return ResponseResult.Failed("Token无效");
                }
            }
            else
            {
                return ResponseResult.Failed("请携带token");
            }
            var user = await _userRepository.FindAsync(userId);
            if (user == null)
            {
                return ResponseResult.Failed("找不到该用户。");
            }
            var IDCard = await _userIDCardQueries.GetFirstSignIdCard(userId);
            if (IDCard == null )
            {
                return ResponseResult.Failed("当前用户未完善身份证号码信息。");
            }

            if (string.IsNullOrEmpty(settlement.WxAppId))
            {
                return ResponseResult.Failed("WxAppId 是必须的。");
            }
            if (string.IsNullOrEmpty(settlement.WxOpenId))
            {
                return ResponseResult.Failed("WxOpenID 是必须的。");
            }

            try
            {
                var result = await _gaoDengService.CreateForBatch(new List<SettlementCreate>()
            {
                new SettlementCreate{
                    name = IDCard.Name,
                    certificate_num = IDCard.Number,
                    certificate_type = CertificateType.ShenFenZheng,
                    phone_num = user.Mobile,
                    wx_appid = settlement.WxAppId,
                    wx_openId = settlement.WxOpenId,
                    settle_amount = settlement.Amount,
                    payment_way = PaymentWay.WeChatPay,
                    order_random_code = settlement.OrderNum,
                    business_scene_code = "YWCJ00028667" //settlement.BusinessSceneCode
                }
            });
                if (result != null && result.code == 0)
                {

                    await _settlementReposiroty.AddAsync(settlement);
                    return ResponseResult.Success();
                }
                else
                {
                    return ResponseResult.Failed(result?.msg);
                }
            }
            catch (GaoDengException)
            {
                return ResponseResult.Failed("提现服务异常，请稍后再来重试~~");
            }

        }

        /// <summary>
        /// 获取H5签约页面。
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ResponseResult> GetSignPage(string returnUrl)
        {
            if (returnUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
            {
                if (!returnUrl.IsSxbUrl())
                {
                    return ResponseResult.Failed("目前仅支持上学帮的跳转路径。");
                }

            }
            var userId = HttpContext.GetUserInfo().UserId;
            var user = await _userRepository.FindAsync(userId);
            if (user == null)
                return ResponseResult.Failed($"Can not found user with {userId}");
            var IDCard = await _userIDCardQueries.GetFirstIdCard(userId);
            if(IDCard == null)
                return ResponseResult.Failed($"Can not found IDCard with {userId}");

            string signPage = _gaoDengService.GetSignPage(new UserBaseInfo()
            {
                idNumber = IDCard.Number,
                certificateType = CertificateType.ShenFenZheng,
                name = IDCard.Name,
                phoneNumber = user.Mobile,
                returnUrl = System.Web.HttpUtility.UrlDecode(returnUrl),
            });

            //清除缓存
            string ThirdCompanySignKey = String.Format("Wallet:ThirdCompanySignUserId_{0}", userId);
            if (await _easyRedisClient.ExistsAsync(ThirdCompanySignKey))
            {
                await _easyRedisClient.RemoveAsync(ThirdCompanySignKey, StackExchange.Redis.CommandFlags.FireAndForget);
            }

            return ResponseResult.Success(new
            {
                signPage
            });
        }


        /// <summary>
        /// 查询结算单状态
        /// </summary>
        /// <param name="orderNum"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetBillStatus(string orderNum)
        {
            var settlement_gaodeng = await _gaoDengService.GetBalance(orderNum, null);
            var settlement = await _settlementReposiroty.FindByOrderNumAsync(settlement_gaodeng.data.order_random_code);
            if (settlement != null)
            {
                var (status, remark) = await SettlementStatusChange(settlement, settlement_gaodeng.data);
                SettlementStatusMessage settlementStatusMessage = new SettlementStatusMessage() { OrderNum = orderNum };
                settlementStatusMessage.Status = status;
                return ResponseResult.Success(settlementStatusMessage, remark);
            }
            else
            {
                _logger.LogWarning("找不到{orderNum}的相关结算单，有可能是创建结算单时发生纰漏，请留意。", orderNum);
                return ResponseResult.Failed("找不到该结算单。");
            }

        }


        /// <summary>
        /// 接受高登结算单状态回调
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReciveAuditResult()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Request.Body))
                {
                    string body = await sr.ReadToEndAsync();
                    _logger.LogDebug(body);
                    var settlements_gaodeng = _gaoDengService.CallBackDecode<List<GaoDeng.Settlement>>(body);
                    if (settlements_gaodeng == null) throw new Exception("Body 解释为空。");
                    foreach (var settlement_gaodeng in settlements_gaodeng)
                    {
                        try
                        {
                            //750 退款成功，5000 已完税。这些都忽略。
                            if (settlement_gaodeng.status == 1004)
                            {
                                //打款失败，做退款操作，让高灯将钱转回余额。
                                await _gaoDengService.RefundBalance(null, new List<string>() { settlement_gaodeng.order_random_code });
                            }
                            var settlement = await _settlementReposiroty.FindByOrderNumAsync(settlement_gaodeng.order_random_code);
                            if (settlement != null)
                            {
                                var (status, remark) = await SettlementStatusChange(settlement, settlement_gaodeng);
                            }

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "高登结算单回调处理失败。{Body}", body);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, null);
            }
            return Content("success");


        }



        /// <summary>
        /// 接受结算单回款通知。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReciveRefundNotify()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Request.Body))
                {
                    string body = await sr.ReadToEndAsync();
                    _logger.LogDebug(body);
                    var refundCallBack = _gaoDengService.CallBackDecode<RefundCallBack>(body);
                    if (refundCallBack == null) throw new Exception("Body 解释为空。");
                    //暂时不用处理。
                    //var settlement = await _settlementReposiroty.FindByOrderNumAsync(refundCallBack.order_random_code);
                    //if (settlement != null && !string.IsNullOrEmpty(settlement.RefundCallBackUrl))
                    //{
                    //    // 回调指定链接，传递参数
                    //    await _httpCallBackNotifyService.NotifySettlementRefundSuccess(settlement.RefundCallBackUrl, new SettlementRefundSuccessMessage()
                    //    {
                    //        OrderNum = refundCallBack.order_random_code,
                    //        RefundAmount = refundCallBack.refund_merchant_amount,
                    //    });
                    //}


                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, null);
            }
            return Content("success");
        }


        /// <summary>
        /// 流转结算单的状态
        /// </summary>
        /// <param name="settlement_gaodeng"></param>
        /// <returns></returns>
        (SettlementStatus, string) TransferSettlementStatusCode(GaoDeng.Settlement settlement_gaodeng)
        {
            SettlementStatus settlementStatus = SettlementStatus.Waiting;
            string remark = null;
            //750 退款成功，5000 已完税。这些都忽略。
            if (settlement_gaodeng.status == 1004)
            {
                //打款失败，做退款操作，让高灯将钱转回余额。
                settlementStatus = SettlementStatus.Fail;
                remark = "打款失败。";
            }
            if (settlement_gaodeng.status == 1000)
            {
                settlementStatus = SettlementStatus.Success;
                remark = "打款成功。";
            }
            if (settlement_gaodeng.status == 5000)
            {
                settlementStatus = SettlementStatus.Success;
                remark = "打款成功并且已完税收。";
            }
            if (settlement_gaodeng.status == 750)
            {
                settlementStatus = SettlementStatus.Fail;
                remark = "打款失败并且回款成功";
            }
            if (settlement_gaodeng.status == 100)
            {
                if (settlement_gaodeng.is_verification)
                {
                    settlementStatus = SettlementStatus.Waiting;
                    remark = "等待中，暂未审核。";
                }
                else
                {
                    settlementStatus = SettlementStatus.Fail;
                    remark = "结算单创建失败，需更正信息后重新创建。";
                }

            }
            if (settlement_gaodeng.status == 200)
            {
                settlementStatus = SettlementStatus.Fail;
                remark = "打款失败，审核不通过。";
            }
            if (settlement_gaodeng.status == 300)
            {
                settlementStatus = SettlementStatus.Waiting;
                remark = "等待中，账户余额不足。";
            }
            if (settlement_gaodeng.status == 600)
            {
                settlementStatus = SettlementStatus.Waiting;
                remark = "等待中，该用户结算量已超过149000元。";
            }
            if (settlement_gaodeng.status == 610)
            {
                settlementStatus = SettlementStatus.Waiting;
                remark = "等待中，该用户还未签约或未认证。";
            }
            return (settlementStatus, remark);


        }

        async Task<(SettlementStatus status, string remark)> SettlementStatusChange(Model.Settlement settlement, GaoDeng.Settlement settlement_gaodeng)
        {
            var (status, remark) = TransferSettlementStatusCode(settlement_gaodeng);
            var oldStatus = settlement.Status;
            settlement.StatusChange(status, remark);
            bool updateFlag = await _settlementReposiroty.UpdateStatusByAsync(settlement.OrderNum, settlement.Status, settlement.Remark);
            if (updateFlag && oldStatus == SettlementStatus.Waiting && (settlement.Status == SettlementStatus.Success || settlement.Status == SettlementStatus.Fail))
            {
                //如果是由等待状态流转到终结状态，通知指定回调。
                if (!string.IsNullOrEmpty(settlement.StatusCallBackUrl))
                {
                    var message = new SettlementStatusMessage()
                    {
                        OrderNum = settlement.OrderNum,
                        Status = settlement.Status,
                        FailReason = remark,
                    };
                    await _httpCallBackNotifyService.NotifySettlementStatus(settlement.StatusCallBackUrl, message);
                    _capPublisher.Publish("SettlementStatusChange", message);
                }

            }
            return (status, remark);
        }
        //[HttpGet]
        //public ResponseResult TestCap()
        //{
        //    _capPublisher.Publish("paysuccess_event",DateTime.Now);
        //    return ResponseResult.Success();
        //}

        //[NonAction]
        //[CapSubscribe("paysuccess_event")]
        //public void CheckReceivedMessage(DateTime datetime)
        //{
        //    Console.WriteLine(datetime);
        //}

        //[NonAction]
        //[CapSubscribe("paysuccess_event",Group ="group2")]
        //public void CheckReceivedMessage2(DateTime datetime)
        //{
        //    Console.WriteLine(datetime);
        //}
    }

}
