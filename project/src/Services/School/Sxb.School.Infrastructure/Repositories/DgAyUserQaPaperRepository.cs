using Dapper;
using Sxb.School.Domain.AggregateModels.DgAyUserQaPaperAggregate;
using Sxb.School.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Infrastructure.Repositories
{
    public class DgAyUserQaPaperRepository : IDgAyUserQaPaperRepository
    {
        public IUnitOfWork UnitOfWork => _dbContext;

        SchoolDataDbContext _dbContext;

        public DgAyUserQaPaperRepository(SchoolDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> UpdateAsync(DgAyUserQaPaper qaPaper, params string[] fields)
        {
            if (fields == null || fields.Any() == false) return false;
            string sql = string.Format(@"UPDATE [dbo].[DgAyUserQaPaper] SET {0}  WHERE id = @id", string.Join(",", fields.Select(f => $"[{f}] = @{f}")));
            return (await _dbContext.Connection.ExecuteAsync(sql, qaPaper, _dbContext.CurrentTransaction)) > 0;

        }

        public async Task<DgAyUserQaPaper> GetAsync(Guid id)
        {
            string sql = @"SELECT [Id]
      ,[UserId]
      ,[Title]
      ,[Atype]
      ,[Status]
      ,[SubmitCount]
      ,[LastSubmitTime]
      ,[AnalyzedTime]
      ,[UnlockedType]
      ,[UnlockedTime]
  FROM [dbo].[DgAyUserQaPaper] WHERE Id = @id";
            var paper = await _dbContext.Connection.QueryFirstOrDefaultAsync<DgAyUserQaPaper>(sql, new { id }, _dbContext.CurrentTransaction);
            return paper;
        }
        public async Task<int?> GetTimesAsync(Guid id)
        {
            string sql = @"
DECLARE @USERID UNIQUEIDENTIFIER 
DECLARE @BTIME DATETIME
DECLARE @ETIME DATETIME
SELECT @USERID = UserId,@BTIME = CAST(UnlockedTime AS date),@ETIME = DATEADD(DAY,1, CAST(UnlockedTime AS date))  FROM DgAyUserQaPaper WHERE ID = @id
select q.id,row_number()over(partition by userid,DATEADD(MS,0,DATEADD(DD, DATEDIFF(DD,0,q.UnlockedTime), 0)) order by UnlockedTime asc)as times
INTO #TEMP
from DgAyUserQaPaper q
where q.IsValid=1 and q.status=3 and q.userid=@USERID  AND Q.UnlockedTime BETWEEN @BTIME AND @ETIME
order by q.UnlockedTime desc , times asc
SELECT times FROM #TEMP WHERE Id = @id";
            return await _dbContext.Connection.ExecuteScalarAsync<int?>(sql, new { id }, _dbContext.CurrentTransaction);
        }
    }
}
