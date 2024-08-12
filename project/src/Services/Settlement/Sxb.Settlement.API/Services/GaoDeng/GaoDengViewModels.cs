using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.GaoDeng
{
    public record ResponseResult
    {
        public int code { get; set; }

        public string msg { get; set; }

        public string request_id { get; set; }

        public string appkey { get; set; }
        public string merchant_id { get; set; }
    }

    public record ResponseResult<T> : ResponseResult
    {
        public T data { get; set; }
    }



    public record Settlement
    {
        public string order_random_code { get; set; }
        public string settlement_code { get; set; }
        /// <summary>
        /// 结算单状态 
        /// 1000 打款成功 
        /// 1004 打款失败
        /// </summary>
        public int status { get; set; }
        public int verification_code { get; set; }
        public decimal service_amount { get; set; }
        public decimal settle_amount { get; set; }
        public decimal total_money { get; set; }
        public string verification_error_code { get; set; }
        public bool is_verification { get; set; }
        public string account_num { get; set; }
        public string fail_reason { get; set; }
        public bool hangup_flag { get; set; }
        public string hangup_msg { get; set; }
        public string hangup_begintime { get; set; }
        public string project_code { get; set; }
        public string pay_time { get; set; }
        public string handle_time { get; set; }
    }

    public record SettlementCreate
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string order_random_code { get; init; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; init; }
        /// <summary>
        /// 1 = 身份证
        /// 3 = 港澳通行证
        /// 4 = 港澳居住证
        /// 5 = 台湾通行证
        /// 6 = 台湾居住证
        /// 9 = 台湾身份证
        /// </summary>
        public CertificateType certificate_type { get; init; }

        public string certificate_num { get; init; }

        public string phone_num { get; init; }

        public string? bank_name { get; init; }
        public string? bankcard_num { get; init; }

        public decimal settle_amount { get; init; }
        /// <summary>
        /// 1 银行卡
        /// 3 支付宝
        /// 9 微信（需要开通微信特约商户）
        /// </summary>
        public PaymentWay payment_way { get; init; }
        /// <summary>
        /// 支付宝时需要填写
        /// </summary>
        public string? payment_account { get; init; }

        public string? alipay_accountid { get; init; }

        /// <summary>
        /// 微信时需要填写
        /// </summary>
        public string? wx_openId { get; init; }


        public string? wx_appid { get; init; }



        /// <summary>
        /// 业务场景编码
        /// </summary>
        public string business_scene_code { get; set; }



    }

    public record UserInfo
    {
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string name { get; init; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string certificate_num { get; init; }

    }

    public record UserBaseInfo
    {
        public string name { get; set; }

        public string idNumber { get; set; }

        public string phoneNumber { get; set; }

        /// <summary>
        /// 结算流水号
        /// </summary>
        public string? serialNum { get; set; }


        /// <summary>
        /// 结算单随机码
        /// </summary>
        public string? orderRandomCode { get; set; }


        /// <summary>
        /// 回调路径
        /// </summary>
        public string? returnUrl { get; set; }

        /// <summary>
        /// 1 = 身份证
        /// 3 = 港澳通行证
        /// 4 = 港澳居住证
        /// 5 = 台湾通行证
        /// 6 = 台湾居住证
        /// 9 = 台湾身份证
        /// </summary>
        public CertificateType certificateType { get; set; } = CertificateType.ShenFenZheng;

    }



    public record RefundCallBack
    {
        public string settlement_code { get; set; }
        public string order_random_code { get; set; }
        /// <summary>
        /// 退回余额
        /// </summary>
        public decimal refund_merchant_amount { get; set; }
        /// <summary>
        /// 服务费抵扣券退回金额
        /// </summary>
        public decimal refund_service_amount { get; set; }
        /// <summary>
        /// 服务费变更单流水号
        /// </summary>
        public string change_code { get; set; }
        /// <summary>
        /// 变更单余额退回余额
        /// </summary>
        public decimal change_merchant_amount { get; set; }
        /// <summary>
        /// 变更单抵扣券退回余额
        /// </summary>
        public decimal change_service_amount { get; set; }

    }

    public record BatchQueryAgreementRequest
    {
        public List<UserInfo> user_infos { get; init; }
    }

    public record AgreementResult
    {
        /// <summary>
        /// 真实名称
        /// </summary>
        public string name { get; init; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string certificate_num { get; init; }

        public bool is_Signed { get; init; }

        /// <summary>
        /// 0-> 尚未签约 1->已签约 2->人工审核中
        /// </summary>
        public int result { get; init; }

        /// <summary>
        /// 签约文件地址
        /// </summary>
        public string agreement_pic { get; init; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public CertificateType certificate_type { get; init; }

    }

    /// <summary>
    /// 1 = 身份证
    /// 3 = 港澳通行证
    /// 4 = 港澳居住证
    /// 5 = 台湾通行证
    /// 6 = 台湾居住证
    /// 9 = 台湾身份证
    /// </summary>
    public enum CertificateType
    {


        /// <summary>
        /// 身份证
        /// </summary>
        ShenFenZheng = 1,
        /// <summary>
        /// 港澳通行证
        /// </summary>
        GanAoTongXingZheng = 3,
        /// <summary>
        /// 港澳居住证
        /// </summary>
        GanAoJuZhuZheng = 4,
        /// <summary>
        /// 台湾通行证
        /// </summary>
        TaiWangTongXingZheng = 5,
        /// <summary>
        /// 台湾居住证
        /// </summary>
        TaiWangJuZhuZheng = 6,

        /// <summary>
        /// 台湾身份证
        /// </summary>
        TaiWangShenFenZheng = 9,

    }

    public enum PaymentWay
    {
        Bank = 1,
        AliPay = 3,
        WeChatPay = 9
    }

    public class GaoDengException : Exception
    {

        public GaoDengException(string message,Exception ex):base(message,ex)
        {

        }
    }
}
