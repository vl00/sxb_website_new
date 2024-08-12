using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Queries.UserSignInInfo
{
    public interface IUserSignInInfoQueries
    {
        Task<IEnumerable<NotifyUserInfoViewModel>> GetNotifyUsers();
        Task<UserSignInInfo> GetUserSignInInfo(Guid userId);
    }
}
