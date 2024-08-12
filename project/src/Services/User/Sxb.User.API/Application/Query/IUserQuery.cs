using Sxb.User.Common.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.Query
{
    public interface IUserQuery
    {
        Task<bool> CheckIsSubscribe(Guid userId);

        /// <summary>是否真实用户</summary>
        Task<bool> IsRealUser(Guid userid);

        /// <summary>用户是否绑定了手机号</summary>
        Task<bool> IsUserBindMobile(Guid userId);

        /// <summary>用户id,名,头像,(达人)描述</summary>
        Task<IEnumerable<UserDescDto>> GetUsersDesc(IEnumerable<Guid> userIds);
        Task<IEnumerable<UserDescDto>> GetUsersDesc2(IEnumerable<Guid> userIds);

        /// <summary>get用户的wx的unionid</summary>
        Task<UserWxUnionIdDto> GetUserWxUnionId(string id);

        /// <summary>
        /// 同学段的达人信息 <br/>
        /// 用户id,名,头像,(达人)描述
        /// </summary>
        Task<IEnumerable<TalentUserDescDto>> GetTopNTalentUserByGrade(int grade, int top);
        /// <summary>
        /// 随机n个虚拟用户 <br/>
        /// 用户id,名,头像,(达人)描述
        /// </summary>
        Task<IEnumerable<UserDescDto>> GetTopNRandVirtualUser(int top);
        Task<IEnumerable<(Guid, string)>> GetNicknames(IEnumerable<Guid> ids);
        Task<IEnumerable<UserWxFwhDto>> GetFwhOpenIdAndNicknames(IEnumerable<Guid> ids);
    }
}