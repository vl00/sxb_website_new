namespace Sxb.School.API.Infrastructures.Services.Models
{
    public class GetSenceQRCodeRequest
    {
        /// <summary>
        /// 0 fwh
        /// </summary>
        public int app { get; set; }

        /// <summary>
        /// 过期秒数
        /// </summary>
        public int expireSecond { get; set; }

        /// <summary>
        /// 扫码关注回调
        /// </summary>
        public string callBackUrl { get; set; }

        /// <summary>
        /// 自定义一些附加信息
        /// </summary>
        public string attach { get; set; }

        public string fw { get; set; }

    }
}
