using Sxb.WenDa.Common.OtherAPIClient.User.Models;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.OtherAPIClient.User
{
    public interface IUserApiService
    {
        /// <summary>是否关注公众号</summary>
        Task<bool> HasGzWxGzh(Guid userid);

        /// <summary>
        /// 是否真实用户
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<bool> IsRealUser(Guid userid);

        /// <summary>用户id,名,头像,(达人)描述</summary>
        Task<IEnumerable<UserDescDto>> GetUsersDesc(IEnumerable<Guid> userIds);

        /// <summary>用户是否绑定了手机号</summary>
        Task<bool> IsUserBindMobile(Guid userId);

        /// <summary>
        /// get用户的wx的unionid
        /// </summary>
        /// <param name="id">userid or unionid</param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取关注用户的用户昵称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<UserWxFwhDto>> GetWxNicknames(IEnumerable<Guid> ids);
    }
}
