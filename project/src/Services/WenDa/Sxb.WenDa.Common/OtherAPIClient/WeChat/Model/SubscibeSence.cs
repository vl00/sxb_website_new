using System.ComponentModel;

namespace Sxb.WenDa.Common.OtherAPIClient.WeChat
{
    public enum SubscibeSence
    {
        [DefaultValue("api/Users/QrCode/Subscribe/CallBack")]
        Subscribe = 0,
        [DefaultValue("api/Users/QrCode/BindWx/CallBack")]
        BindWx = 1,
    }
}