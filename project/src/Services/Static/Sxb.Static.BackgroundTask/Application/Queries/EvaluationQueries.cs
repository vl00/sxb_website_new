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
    public class EvaluationQueries : IEvaluationQueries
    {
        private readonly string _connectionString;
        public EvaluationQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString)); ;
        }


        public async Task<Evaluation> GetEvaluationFromNoAsync(long no)
        {
            using (IDbConnection connection = new SqlConnection(this._connectionString))
            {
                Evaluation evaluation = await Policy.Handle<Exception>()
                .WaitAndRetryAsync(new[] {
                 TimeSpan.FromSeconds(1),
                 TimeSpan.FromSeconds(5)
                }, (exception, ts) =>
                {
                    Console.WriteLine("处理错误重试:" + exception.Message);
                })
                .ExecuteAsync(async () =>
                {
                    return await connection.QueryFirstOrDefaultAsync<Evaluation>(@"
SELECT id
  FROM [Organization].[dbo].[Evaluation]
WHERE No=@no
", new { no = no });

                });

                if (evaluation == null)
                    throw new KeyNotFoundException();
                return evaluation;
            }

        }
    }
}
