using Sxb.User.Common.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.User.Query.SQL.IRepository
{
    public interface IUserRepository
    {
        Task<bool> GetSubscribe(Guid userId);

        Task<bool> IsRealUser(Guid userid);

        Task<bool> IsUserBindMobile(Guid userId);

        Task<IEnumerable<UserDescDto>> GetUsersDesc(IEnumerable<Guid> userIds);
        Task<IEnumerable<UserDescDto>> GetUsersDesc2(IEnumerable<Guid> userIds);

        Task<UserWxUnionIdDto> GetUserWxUnionId(Guid userId = default, string unionId = default);

        Task<IEnumerable<(Guid UserId, bool IsInternal)>> GetTopNTalentUserIdByGrade(int grade, int top);
        Task<IEnumerable<Guid>> GetTopNRandVirtualUserId(int top);
        Task<IEnumerable<(Guid, string)>> GetNicknames(IEnumerable<Guid> ids);
        Task<IEnumerable<UserWxFwhDto>> GetFwhOpenIdAndNicknames(IEnumerable<Guid> ids);
    }
}