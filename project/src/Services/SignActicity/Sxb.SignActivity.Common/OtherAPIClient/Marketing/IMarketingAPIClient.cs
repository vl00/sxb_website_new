using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.SignActivity.Common.OtherAPIClient.Marketing.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.OtherAPIClient.Marketing
{
    public interface IMarketingAPIClient
    {
        /// <summary>
        /// 获取所有普通顾问和高级顾问的userId
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetConsultantUserIds();
        Task<FxUserResponse> GetFxUser(Guid userId);
        Task<PreLockFxUserResponse> GetPreLockFxUser(Guid userId);
        Task<UserWxOpenInfo> GetWxOpenInfo(Guid userId);
    }
}