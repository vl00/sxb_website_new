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
    public class TalentQueries : ITalentQueries
    {
        private readonly string _connectionString;
        public TalentQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString)); ;
        }

        public async Task<Talent> GetTalentFromUserIdAsync(Guid userId)
        {
            using (IDbConnection connection = new SqlConnection(this._connectionString))
            {
                Talent talent = await Policy.Handle<Exception>()
                .WaitAndRetryAsync(new[] {
                 TimeSpan.FromSeconds(1),
                 TimeSpan.FromSeconds(5)
                }, (exception, ts) =>
                {
                    Console.WriteLine("处理错误重试:" + exception.Message);
                })
                .ExecuteAsync(async () =>
                {
                    return await connection.QueryFirstOrDefaultAsync<Talent>(@"
SELECT talent.id,userInfo.city FROM talent
JOIN userInfo ON userInfo.id =  talent.user_id
WHERE
talent.user_id= @userId
", new { userId = userId });

                });

                if (talent == null)
                    throw new KeyNotFoundException($"key={userId}");
                return talent;
            }

        }
    }
}
