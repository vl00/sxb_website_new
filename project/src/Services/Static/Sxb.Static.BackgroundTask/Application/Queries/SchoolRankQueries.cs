using Dapper;
using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Application.Queries
{
    public class SchoolRankQueries: ISchoolRankQueries
    {

        private readonly string _connectionString;
        public SchoolRankQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString)); ;
        }

        public async Task<SchoolRank> GetSchoolRankFromNoAsync(long no)
        {
            using (IDbConnection connection = new SqlConnection(this._connectionString))
            {
                SchoolRank schoolRank = await Policy.Handle<Exception>()
                .WaitAndRetryAsync(new[] {
                 TimeSpan.FromSeconds(1),
                 TimeSpan.FromSeconds(5)
                }, (exception, ts) =>
                {
                    Console.WriteLine("处理错误重试:" + exception.Message);
                })
                .ExecuteAsync(async () =>
                {
                    var schoolRankDynamic = await connection.QueryFirstOrDefaultAsync(@"
SELECT SchoolRank.Id id
,(SELECT  CityId  FROM SchoolRankAreaBinds where  SchoolRank.Id = SchoolRankAreaBinds.SchoolRankId  FOR JSON PATH) citys
FROM SchoolRank WHERE No = @no
", new { no = no });
                    if (schoolRankDynamic != null)
                    {
                        List<int> citys = new List<int>();
                        if (!string.IsNullOrEmpty(schoolRankDynamic.citys))
                        {
                            citys.AddRange(JArray.Parse((string)schoolRankDynamic.citys).Select(t =>
                            {
                                var cityId = t["CityId"]?.Value<int?>();
                                return cityId.GetValueOrDefault();
                            }));
                        }
                        return new SchoolRank()
                        {
                            Id = schoolRankDynamic.id,
                            Citys = citys
                        };
                    }
                    else
                    {
                        return null;
                    }

                });

                if (schoolRank == null)
                    throw new KeyNotFoundException();
                return schoolRank;
            }

        }
    }
}
