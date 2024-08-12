using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.WenDa.Common.Enum;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.DB;
using System.Linq;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public class SchoolQuestionRepository : ISchoolQuestionRepository
    {
        readonly LocalQueryDB _queryDB;

        public SchoolQuestionRepository(LocalQueryDB queryDB)
        {
            _queryDB = queryDB;
        }

        /// <summary>
        /// 热门学校问答
        /// 
        /// 默认显示用户城市定位下的所有幼小初高中问答数量超过5条的学校问答。
        /// 优先显示学校问答数量多的学校。
        /// 问答按时间从近到远排序。
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Guid>> GetHotSchoolIdsAsync(ArticlePlatform platform, int top)
        {
            //Dapper.SqlMapper.Aop.OnExecuting += Aop_OnExecuting;

            var platformValue = platform.GetDefaultValue<long>();
            var platformSql = platform == ArticlePlatform.Master ? "" : "and Q.Platform = @platformValue";
            var sql = $@"
SELECT 
    top {top}
	QE.EId
FROM 
	Question Q
	INNER JOIN dbo.QuestionEids QE ON QE.QId = Q.Id
WHERE 
	Q.IsValid = 1 
    {platformSql}
GROUP BY 
	QE.EId
ORDER BY
	COUNT(1) DESC
";
            return await _queryDB.SlaveConnection.QueryAsync<Guid>(sql, new { platformValue });
        }

        private void Aop_OnExecuting(ref CommandDefinition command)
        {

        }
    }
}
