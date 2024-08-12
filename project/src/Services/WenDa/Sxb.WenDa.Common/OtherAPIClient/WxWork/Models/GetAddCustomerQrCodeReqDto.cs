namespace Sxb.WenDa.Common.OtherAPIClient.WxWork.Models
{
    public class GetAddCustomerQrCodeReqDto
    {
        /// <summary>
        /// wx unionid
        /// </summary>
        public string OpenId { get; set; }

        public string NotifyUrl { get; set; }

        public string AttachData { get; set; }

        public string Scene { get; set; } = "Wenda";
    }

    public class GetAddCustomerQrCodeResDto
    {
        /// <summary>是否已加企业微信客服</summary>
        public bool HasJoinWxEnt => string.IsNullOrEmpty(QrcodeUrl);
        /// <summary>
        /// 未加企业微信客服时返回二维码
        /// </summary>
        public string QrcodeUrl { get; set; }
    }
}
