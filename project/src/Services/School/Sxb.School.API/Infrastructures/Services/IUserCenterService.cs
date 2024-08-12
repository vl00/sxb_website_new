using System;
using System.Threading.Tasks;

namespace Sxb.School.API.Infrastructures.Services
{
    public interface IUserCenterService
    {
        Task<string> GetLoginCodeAsync(Guid userId);
    }
}
