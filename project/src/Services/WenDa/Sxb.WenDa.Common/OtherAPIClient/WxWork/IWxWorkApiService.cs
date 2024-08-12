using Sxb.WenDa.Common.OtherAPIClient.WxWork.Models;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.OtherAPIClient.WxWork
{
    public interface IWxWorkApiService
    {
        /// <summary>
        /// 用户是否加入企业wx
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<GetAddCustomerQrCodeResDto> GetAddCustomerQrCode(GetAddCustomerQrCodeReqDto dto);
    }
}
