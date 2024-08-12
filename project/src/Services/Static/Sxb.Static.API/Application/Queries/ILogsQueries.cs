using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Static.API.Application.Queries
{
    public interface ILogsQueries
    {

        Task<IEnumerable<LogData>> QuerySchoolLogDatasAsync(int? city, DateTime st, DateTime et);
        Task<IEnumerable<LogData>> QueryTalentLogDatasAsync(int? city, DateTime st, DateTime et);
        Task<IEnumerable<LogData>> QueryCommentLogDatasAsync(int? city, DateTime st, DateTime et);
        Task<IEnumerable<LogData>> QueryEvaluationLogDatasAsync(DateTime st, DateTime et);
        Task<IEnumerable<LogData>> QueryLiveLogDatasAsync(int? city, DateTime st, DateTime et);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">1 -> 课程， 2->好物</param>
        /// <param name="st"></param>
        /// <param name="et"></param>
        /// <returns></returns>
        Task<IEnumerable<LogData>> QueryCourseLogDatasAsync(int type, DateTime st, DateTime et);

        Task<IEnumerable<LogData>> QueryQuestionLogDatasAsync(int? city, DateTime st, DateTime et);

        Task<IEnumerable<LogData>> QueryArticleLogDatasAsync(int? city, DateTime st, DateTime et);

        Task<IEnumerable<LogData>> QuerySchoolRankLogDatasAsync(int? city, DateTime st, DateTime et);

        Task<IEnumerable<LogData>> QueryTopicLogDatasAsync(DateTime st, DateTime et);

    }


}
