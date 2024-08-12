using Dapper;
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
    public class QuestionQueries: IQuestionQueries
    {
        private readonly string _connectionString;
        public QuestionQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString)) ;
        }

        public async Task<Question> GetQuestionFromNoAsync(long no)
        {
            using (IDbConnection connection = new SqlConnection(this._connectionString))
            {
                Question question = await Policy.Handle<Exception>()
                .WaitAndRetryAsync(new[] {
                 TimeSpan.FromSeconds(1),
                 TimeSpan.FromSeconds(5)
                }, (exception, ts) =>
                {
                    Console.WriteLine("处理错误重试:" + exception.Message);
                })
                .ExecuteAsync(async () =>
                {
                    return await connection.QueryFirstOrDefaultAsync<Question>(@"
  SELECT   [QuestionInfos].Id,
  OnlineSchoolExtContent.province,
  OnlineSchoolExtContent.city,
  OnlineSchoolExtContent.area
  FROM [iSchoolProduct].[dbo].[QuestionInfos]
  JOIN iSchoolData.dbo.OnlineSchoolExtContent ON [QuestionInfos].SchoolSectionId = OnlineSchoolExtContent.eid
  WHERE [no] = @no
", new { no = no });

                });

                if (question == null)
                    throw new KeyNotFoundException();
                return question;
            }

        }
    }
}
