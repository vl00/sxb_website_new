using Sxb.PaidQA.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.PaidQA.Query.SQL.IRepository
{
    public interface ITalentRepository
    {
        /// <summary>
        /// 获取达人付费问答设置
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<TalentSettingInfo> GetTalentSetting(Guid userID);
        /// <summary>
        /// 根据学段获取
        /// </summary>
        /// <param name="gradeIndex">学段Index</param>
        /// <param name="isInternal">是否内部达人</param>
        /// <returns></returns>
        Task<IEnumerable<TalentSettingInfo>> GetTalentSettingsByGradeIndex(int gradeIndex, bool isInternal = false);
        /// <summary>
        /// 获取达人的学段
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<IEnumerable<GradeInfo>> GetTalentGrade(Guid userID);
        /// <summary>
        /// 获取达人擅长领域
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<IEnumerable<RegionTypeInfo>> GetTalentRegions(Guid userID);
        /// <summary>
        /// 获取达人付费问答等级
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<string> GetTalentLevelName(Guid userID);
        /// <summary>
        /// 获取学部绑定的达人用户ID
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<Guid> GetExtensionTalentUserIDAsync(Guid eid);
    }
}
