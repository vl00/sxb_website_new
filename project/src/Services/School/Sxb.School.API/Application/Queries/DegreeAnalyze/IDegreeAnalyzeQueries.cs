using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sxb.School.API.RequestContact.DegreeAnalyze;
using Sxb.School.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Queries.DegreeAnalyze
{
    /// <summary>
    /// 2022学位分析器
    /// </summary>
    public interface IDegreeAnalyzeQueries
    {
        /// <summary>获取题目s</summary>
        Task<DgAyGetQuestionResponse> GetQuestions(bool readCache = true, bool writeCache = true, bool useReadConn = true, bool showFindField = false);

        /// <summary>
        /// 题型type=5时,根据选择了的地区,获取具体地址
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        Task<DgAyGetQuesAddressesResponse> GetQuesAddresses(int area);
        Task<PagedList<string>> GetQuesAddresses(DgAyFindAddressesQuery query);

        Task<PagedList<DgAyMyQaResultListItem>> GetMyQaResultList(Guid userid, int pageIndex, int pageSize);

        /// <summary>
        /// 我的分析报告结果页
        /// </summary>
        Task<DgAyQaResultVm> GetQaResult(Guid id, bool showAll = false, Guid? me = null, bool includeDeletedSchool = false);
        /// <summary>
        /// 我的答题情况(仅仅答题内容)
        /// </summary>
        Task<DgAyQaResultVm0> GetQaCtn(Guid id);

        /// <summary>
        /// 我的分析报告是否已解锁
        /// </summary>
        Task<DgAyQaIsUnlockedVm> IsUnlocked(Guid id, Guid? me = null);
    }
}
