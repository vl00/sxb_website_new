using Sxb.School.API.Infrastructures.Services.Models;
using System.Threading.Tasks;

namespace Sxb.School.API.Infrastructures.Services
{
    public interface IPayGatewayService
    {
        Task<(string orderNo, string orderId)> PayByH5(AddPayOrderRequest request);
    }
}
