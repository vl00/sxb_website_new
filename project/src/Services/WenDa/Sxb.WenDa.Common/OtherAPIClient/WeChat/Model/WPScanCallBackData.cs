namespace Sxb.WenDa.Common.OtherAPIClient.WeChat
{
    public class WPScanCallBackData
    {
        public string AppId { get; set; }
        public string OpenId { get; set; }
        public string UnionId { get; set; }
        public bool IsSubscribe { get; set; }
        public int SubscribeTime { get; set; }
        public string Attach { get; set; }

        public Guid UserId { get; set; }

    }
}