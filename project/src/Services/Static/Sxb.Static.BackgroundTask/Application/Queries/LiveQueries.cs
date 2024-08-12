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
    public class LiveQueries : ILiveQueries
    {
        private readonly string _connectionString;
        public LiveQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString)); ;
        }


        public async Task<Live> GetLiveFromIdAsync(Guid id)
        {
            using (IDbConnection connection = new SqlConnection(this._connectionString))
            {
                Live live = await Policy.Handle<Exception>()
                .WaitAndRetryAsync(new[] {
                 TimeSpan.FromSeconds(1),
                 TimeSpan.FromSeconds(5)
                }, (exception, ts) =>
                {
                    Console.WriteLine("处理错误重试:" + exception.Message);
                })
                .ExecuteAsync(async () =>
                {
                    return await connection.QueryFirstOrDefaultAsync<Live>(@"
SELECT 
id,
city
FROM [iSchoolLive].[dbo].[lecture_v2] 
WHERE  id = @id
", new { id = id });

                });

                if (live == null)
                    throw new KeyNotFoundException();
                return live;
            }

        }
    }
}
