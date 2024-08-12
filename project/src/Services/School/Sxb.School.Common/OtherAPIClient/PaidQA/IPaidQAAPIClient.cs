using Sxb.School.Common.OtherAPIClient.PaidQA.Model.EntityExtend;
using System;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.PaidQA
{
    public interface IPaidQAAPIClient
    {
        /// <summary>
        /// 根据用户ID获取达人
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<TalentDetailExtend> GetTalentByUserID(Guid userID);
        /// <summary>
        /// 根据学段随机一个达人
        /// </summary>
        /// <param name="grade">学段</param>
        /// <param name="isInternal">是否内部达人</param>
        /// <returns></returns>
        Task<TalentDetailExtend> RandomTalentByGrade(int grade, bool isInternal = false, Guid? eid = null);
    }
}
