using Sxb.PointsMall.Domain.AggregatesModel.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Domain.AggregatesModel.UserSignInInfoAggregate
{
    public interface IUserSignInInfoRepository : IRepository<UserSignInInfo>
    {

        Task AddAsync(UserSignInInfo  userSignInInfo);

        Task<bool> UpdateAsync(UserSignInInfo  userSignInInfo, params string[] fields);


        Task<UserSignInInfo> FindFromAsync(Guid userId);


        /// <summary>
        /// 自校验性更新连续天数
        /// </summary>
        /// <param name="userSignInInfo"></param>
        /// <returns></returns>
        Task<bool> UpdateContinueDaysAsync(UserSignInInfo userSignInInfo);

    }
}
