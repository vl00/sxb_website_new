using Sxb.PaidQA.Common.EntityExtend;
using System;
using System.Threading.Tasks;

namespace Sxb.PaidQA.API.Application.Query
{
    public interface ITalentQuery
    {
        /// <summary>
        /// 获取达人详情
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<TalentDetailExtend> GetDetail(Guid userID);
        /// <summary>
        /// 随机一个学段达人
        /// </summary>
        /// <param name="grade">学段</param>
        /// <param name="isInternal">是否内部达人</param>
        /// <returns></returns>
        Task<TalentDetailExtend> RandomByGrade(int grade, bool isInternal = false);
        /// <summary>
        /// 获取学部绑定的达人用户ID
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<Guid> GetExtensionTalentUserIDAsync(Guid eid);
    }
}
