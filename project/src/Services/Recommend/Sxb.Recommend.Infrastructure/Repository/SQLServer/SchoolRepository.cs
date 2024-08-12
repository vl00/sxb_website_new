using Microsoft.Data.SqlClient;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using Sxb.Recommend.Infrastructure.Repository.SQLServer.DataBases;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.Repository.SQLServer
{
    public class SchoolRepository : ISchoolRepository
    {
        DataBase _db;

        public SchoolRepository(ISchoolDataDB iSchoolDataDB)
        {
            _db = iSchoolDataDB;
        }


        public async Task<IEnumerable<School>> GetValidAfterAsync(DateTime preUpdateTime, int offset, int limit)
        {
            string sql = @"SELECT 
OnlineSchoolExtension.id
,OnlineSchoolExtension.type
,OnlineSchoolExtension.schFtype 
,OnlineSchoolExtension.isValid
,OnlineSchool.status
INTO #temp
FROM OnlineSchoolExtension
LEFT JOIN OnlineSchool ON OnlineSchool.id = OnlineSchoolExtension.sid
WHERE OnlineSchoolExtension.ModifyDateTime > @preUpdateTime
ORDER BY OnlineSchoolExtension.id
OFFSET @offset ROWS
FETCH NEXT @limit ROW ONLY
SELECT 
OnlineSchoolExtension.*
,(SELECT TOP 1 score FROM Score 
			WHERE indexid = 22 AND status=1 AND eid =OnlineSchoolExtension.id ) score
,(SELECT TOP 1 courses FROM OnlineSchoolExtCourse WHERE OnlineSchoolExtCourse.eid = OnlineSchoolExtension.id )  courses
,(SELECT TOP 1 city FROM OnlineSchoolExtContent WHERE  OnlineSchoolExtContent.eid = OnlineSchoolExtension.id) city
,(SELECT TOP 1 area FROM OnlineSchoolExtContent WHERE  OnlineSchoolExtContent.eid = OnlineSchoolExtension.id) area
,(SELECT TOP 1 [authentication] FROM OnlineSchoolExtContent WHERE  OnlineSchoolExtContent.eid = OnlineSchoolExtension.id) [authentication]
,(SELECT TOP 1 characteristic FROM OnlineSchoolExtContent WHERE  OnlineSchoolExtContent.eid = OnlineSchoolExtension.id) characteristic
FROM #temp OnlineSchoolExtension

";
            var dbores = await _db.Connection.QueryAsync(sql, new { preUpdateTime = preUpdateTime, offset, limit }, null, (int)TimeSpan.FromHours(1).TotalSeconds);
            if (dbores?.Any() == true)
            {
                return dbores.Select(s =>
                {
                    IEnumerable<string> authentications = new List<string>();
                    if (!string.IsNullOrEmpty(s.authentication) && s.authentication != "[]")
                    {
                        authentications = JArray.Parse((string)s.authentication).Select(t => t["Key"].Value<string>());
                    }
                    IEnumerable<string> courses = new List<string>();
                    if (!string.IsNullOrEmpty(s.courses) && s.courses != "[]")
                    {
                        courses = JArray.Parse((string)s.courses).Select(t => t["Key"].Value<string>());
                    }
                    IEnumerable<string> characteristics = new List<string>();
                    if (!string.IsNullOrEmpty(s.characteristic) && s.characteristic != "[]")
                    {
                        characteristics = JArray.Parse((string)s.characteristic).Select(t => t["Key"].Value<string>());
                    }

                    return new School(
                        s.id
                        , s.area ?? 0
                        , s.type ?? 0
                        , s.score ?? 0
                        , authentications
                        , courses
                        , characteristics
                        , s.city ?? 0
                        , s.schFtype
                        , s.isValid ?? false
                        , s.status ?? 0
                        );
                });
            }
            else
            {
                return new List<School>();
            }

        }

        public async Task<(Guid id1, Guid id2)> GetIdByNo(long no1, long no2)
        {
            var sql = $@" 
select Id from OnlineSchoolExtension where no = @no1
select Id from OnlineSchoolExtension where no = @no2
";
            using var reader = await _db.Connection.QueryMultipleAsync(sql, new { no1, no2 });
            var id1 = await reader.ReadFirstOrDefaultAsync<Guid>();
            var id2 = await reader.ReadFirstOrDefaultAsync<Guid>();
            return (id1, id2);
        }
    }
}
