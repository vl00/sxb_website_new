namespace Sxb.WenDa.Common.OtherAPIClient.WeChat
{
    public class WPScanRequestData
    {

        /// <summary>
        ///  0 服务号
        /// </summary>
        public int App { get; set; } = 0;

        /// <summary>
        /// 过期时间 最大30天
        /// </summary>
        public int ExpireSecond { get; set; } = (int)TimeSpan.FromDays(30).TotalSeconds; //30 * 24 * 3600

        /// <summary>
        /// 回调地址
        /// </summary>
        public string CallBackUrl { get; set; }

        /// <summary>
        /// 附加信息
        /// </summary>
        public string Attach { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Fw { get; set; }
    }
}