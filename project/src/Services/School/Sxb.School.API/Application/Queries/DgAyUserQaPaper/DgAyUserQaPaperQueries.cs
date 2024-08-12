using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Queries.DgAyUserQaPaper
{
    using Dapper;
    using System.Linq;

    public class DgAyUserQaPaperQueries : IDgAyUserQaPaperQueries
    {
        private readonly string _connectionStr;
        public DgAyUserQaPaperQueries(string connectionString)
        {
            _connectionStr = connectionString;
        }

        public async Task<List<Guid>> GetALevelEIds(Guid id)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"
SELECT j.eid FROM DgAyUserQaPaper P
JOIN  [dbo].[DgAyUserQaResultContent] C ON  C.Qaid = P.Id
outer apply openjson(C.eids)  with(eid uniqueidentifier '$') j
WHERE 
P.ID = @id
and 
exists(select 1 from SchoolViewALevel where ID = j.eid )
";
            var res = await con.QueryAsync(sql, new { id });
            return res.Select(s => (Guid)s.eid).ToList();
        }

        public async Task<List<Guid>> GetALevelEIdsWithUnOpenPermission(Guid id)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"
SELECT j.eid FROM DgAyUserQaPaper P
JOIN  [dbo].[DgAyUserQaResultContent] C ON  C.Qaid = P.Id
outer apply openjson(C.eids)  with(eid uniqueidentifier '$') j
WHERE 
P.ID = @id
and 
exists(select 1 from SchoolViewALevel where ID = j.eid )
AND
NOT EXISTS(SELECT 1 FROM SchoolViewPermission WHERE P.UserId = UserId AND J.EID = EXTID)
";
            var res = await con.QueryAsync(sql, new { id });
            return res.Select(s => (Guid)s.eid).ToList();
        }

        public async Task<DgAyUserQaPaper> GetDgAyUserQaPaper(Guid id)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"SELECT [Id]
      ,[UserId]
      ,[CreateTime]
      ,[Title]
      ,[Atype]
      ,[Status]
      ,[SubmitCount]
      ,[LastSubmitTime]
      ,[AnalyzedTime]
      ,[UnlockedType]
      ,[UnlockedTime]
      ,[IsValid]
  FROM [dbo].[DgAyUserQaPaper]
  WHERE ID = @id
";
            var paper = await con.QueryFirstOrDefaultAsync<DgAyUserQaPaper>(sql, new { id });
            if (paper == null)
                throw new KeyNotFoundException($"找不到Paper,paperId={id}");
            return paper;
        }
    }
}
