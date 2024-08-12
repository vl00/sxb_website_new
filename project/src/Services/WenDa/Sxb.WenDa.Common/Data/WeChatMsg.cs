namespace Sxb.WenDa.Common.Data
{
    public class WeChatMsg
    {
        public WeChatMsgItem BindAccountKFMsg { get; set; }
        public WeChatMsgItem WelcomKFMsg { get; set; }
    }

    public class WeChatMsgItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string RedirectUrl { get; set; }
        public string ImgUrl { get; set; }
    }
}