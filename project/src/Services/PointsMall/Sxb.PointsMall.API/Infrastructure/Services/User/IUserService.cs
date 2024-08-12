using System;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Infrastructure.Services
{
    public interface IUserService
    {
        Task<bool> GetUserSubscribe(Guid userId);
    }
}