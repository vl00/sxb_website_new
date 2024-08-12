namespace Sxb.WenDa.Common.OtherAPIClient.WeChat
{
    public class WxMessageRequest<T> where T : class
    {
        /// <summary>
        ///  0 服务号
        /// </summary>
        public int App { get; set; } = 0;
        public T NewsCustomMsg { get; set; }
    }
}