using Sxb.School.API.Infrastructures.Services.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sxb.School.API.Infrastructures.Services
{
    public interface IWeChatGatewayService
    {
        Task<string> GetSenceQRCode(GetSenceQRCodeRequest request);
        Task SendSendTextMsg(string toUser, string content);
        Task SendNewsMsg(string toUser, string title, string description, string url, string picUrl);
        Task SendImgMsg(string toUser, Stream file);

        Task SendTplMsg(string toUser,string tplId,string url ,List<TplDataFiled> fileds);
    }
}
