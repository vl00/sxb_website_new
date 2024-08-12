using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.OtherAPIClient.WeChat
{
    public interface IWxChatApiService
    {
        Task<WxApiResult<string>> GetSenceQRCode(WPScanRequestData requestData);
        Task<WxApiResult<object>> SendNewsMsg(NewsCustomMsg requestData);
    }
}