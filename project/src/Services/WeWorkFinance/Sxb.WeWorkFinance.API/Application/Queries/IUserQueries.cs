using Sxb.WeWorkFinance.API.Application.Queries.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Queries
{
    public interface IUserQueries
    {
        Task<UserInfoViewModel> GetUserInfo(string unionId);
        Task<bool> AddUserInfo(string unionId, Guid userId, string nickName, int? sex, string headImgUrl);
        Task<List<UserInfoViewModel>> GetUserFwhOpenId(List<string> unionIds);
    }
}
