using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Polly;

namespace Sxb.Static.BackgroundTask.Application.Queries
{
    public class SchoolQueries : ISchoolQueries
    {
        private readonly string _connectionString;
        public SchoolQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString)); ;
        }

        public async Task<School> GetSchoolFromNoAsync(long no)
        {
            using (IDbConnection connection = new SqlConnection(this._connectionString))
            {
                School school = await Policy.Handle<Exception>()
                .WaitAndRetryAsync(new[] {
                 TimeSpan.FromSeconds(1),
                 TimeSpan.FromSeconds(5)
                }, (exception, ts) =>
                {
                    Console.WriteLine("处理错误重试:" + exception.Message);
                })
                .ExecuteAsync(async () =>
                {
                    return await connection.QueryFirstOrDefaultAsync<School>(@"
SELECT OnlineSchoolExtension.id ExtId,province,city,area from OnlineSchoolExtension
JOIN OnlineSchoolExtContent ON  OnlineSchoolExtension.id = OnlineSchoolExtContent.eid
WHERE OnlineSchoolExtension.No=@no
", new { no = no });

                });

                if (school == null)
                    throw new KeyNotFoundException();
                return school;
            }

        }
    }
}
