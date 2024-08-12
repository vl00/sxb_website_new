using Sxb.School.API.Infrastructures.Services.Models;
using System.Threading.Tasks;

namespace Sxb.School.API.Infrastructures.Services
{
    public interface IWxWorkService
    {
        Task<GetAddCustomerQrCodeResponse> GetAddCustomerQrCode(GetAddCustomerQrCodeRequest request);
    }
}
