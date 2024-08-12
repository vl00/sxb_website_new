using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.OtherAPIClient.Marketing
{
    public interface IWeChatAPIClient
    {
        Task<string> GetAccessToken();
    }
}