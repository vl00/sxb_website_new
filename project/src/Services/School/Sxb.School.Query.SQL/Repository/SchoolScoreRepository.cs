using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.DB;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.Repository
{
    public class SchoolScoreRepository : ISchoolScoreRepository
    {
        readonly SchoolDataDB _schoolDataDB;
        public SchoolScoreRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<IEnumerable<ScoreIndexInfo>> GetAllScoreIndexs()
        {
            return await _schoolDataDB.SlaveConnection.QuerySet<ScoreIndexInfo>().ToIEnumerableAsync();
        }

        public async Task<IEnumerable<SchoolExtensionScoreInfo>> GetExtensionScores(Guid eid)
        {
            if (eid == default) return null;
            return await _schoolDataDB.SlaveConnection.QuerySet<SchoolExtensionScoreInfo>().Where(p => p.EID == eid && p.Status == true).ToIEnumerableAsync();
        }

        public async Task<double> GetSchoolRankingInCity(int cityCode, double score, string schFType)
        {
            var str_AllCountSQL = $@"SELECT
	                                Count(s.id) 
                                FROM
	                                Score AS s
	                                LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = s.eid 
	                                LEFT JOIN OnlineSchoolExtension as ose on ose.id = s.eid
                                WHERE
	                                s.indexid = 22 
	                                AND ose.SchFtype = @schFType
	                                AND osec.city = @cityCode;";
            var allCount = await _schoolDataDB.SlaveConnection.QuerySingleAsync<int>(str_AllCountSQL, new { cityCode, schFType });
            var str_ScoreCountSQL = $@"SELECT
	                                        Count(s.id) 
                                        FROM
	                                        Score AS s
	                                        LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = s.eid
	                                        LEFT JOIN OnlineSchoolExtension as ose on ose.id = s.eid
                                        WHERE
	                                        s.indexid = 22
	                                        AND ose.SchFtype = @schFType
	                                        AND osec.city = @cityCode
	                                        AND s.score >= @score";
            var scoreCount = await _schoolDataDB.SlaveConnection.QuerySingleAsync<int>(str_ScoreCountSQL, new { cityCode, score, schFType });
            return (scoreCount / (allCount + 0.0));
        }

        public async Task<IEnumerable<Guid>> GetEidsForSchoolScoreTopNByGrade(int top, int grade)
        {
            var sql = $@"
                    select top {top} sc.eid,sc.score 
                    from ScoreTotal sc 
                    left join OnlineSchoolExtension e on sc.eid=e.id and e.IsValid=1
                    left join OnlineSchool s on s.id=e.sid and s.isvalid=1 and s.status=3
                    where 1=1 {(grade == 0 ? "" : "and e.grade=@grade")}
                        and sc.status=1
                    order by sc.score desc
                ";
            var ls = await _schoolDataDB.SlaveConnection.QueryAsync<(Guid, double)>(sql, new { grade });
            return ls.Select(_ => _.Item1).ToList();
        }


        public async Task<IEnumerable<(Guid Eid, double Score)>> GetSchoolsScoreEids(IEnumerable<Guid> eids)
        {
            var sql = $@"
                    select sc.eid,sc.score 
                    from ScoreTotal sc
                    where 1=1 and sc.status=1 and sc.eid in @eids                    
                ";
            var ls = await _schoolDataDB.SlaveConnection.QueryAsync<(Guid, double)>(sql, new { eids });
            return ls;
        }

    }
}
