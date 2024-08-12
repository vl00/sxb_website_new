using Dapper;
using Sxb.PointsMall.Domain.AggregatesModel.UserSignInInfoAggregate;
using Sxb.PointsMall.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Infrastructure.Repositories
{
    public class UserSignInInfoRepository : IUserSignInInfoRepository
    {
        public IUnitOfWork UnitOfWork => _dbContext;

        PointsMallDbContext _dbContext;

        public UserSignInInfoRepository(PointsMallDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(UserSignInInfo userSignInInfo)
        {
            string sql = @"INSERT INTO [dbo].[UserSignInInfos]
           ([Id]
           ,[UserId]
           ,[EnableNotify]
           ,[ContinueDays]
           ,[PreContinueDays]
           ,[ModifyTime]
           ,[LastSignDate])
SELECT 
            @Id
           ,@UserId
           ,@EnableNotify
           ,@ContinueDays
           ,@PreContinueDays
           ,@ModifyTime
           ,@LastSignDate
WHERE  NOT EXISTS(SELECT 1 FROM  [dbo].[UserSignInInfos] WHERE UserId = @UserId)"; //SQL层级为 UserId加唯一键约束
            await _dbContext.Connection.ExecuteAsync(sql, userSignInInfo, _dbContext.CurrentTransaction);
        }

        public async Task<bool> UpdateAsync(UserSignInInfo accountPoints, params string[] fields)
        {
            if (fields == null || !fields.Any()) throw new ArgumentNullException($"{nameof(fields)} is empty.");
            string upTemplate = "Update {0} Set {1} Where Id = @Id";
            IEnumerable<string> sets = fields.Select(f => $"[{f}]= @{f}");
            string sql = string.Format(upTemplate, "[dbo].[UserSignInInfos]", string.Join(",", sets));
            return (await _dbContext.Connection.ExecuteAsync(sql, accountPoints, _dbContext.CurrentTransaction)) > 0;
        }

        public async Task<UserSignInInfo> FindFromAsync(Guid userId)
        {
            string sql = @"
SELECT [Id]
      ,[UserId]
      ,[EnableNotify]
      ,[ContinueDays]
      ,[PreContinueDays]
      ,[ModifyTime]
      ,[LastSignDate]
  FROM [dbo].[UserSignInInfos]
WHERE UserId = @userId";
            var res = await _dbContext.Connection.QueryFirstOrDefaultAsync<UserSignInInfo>(sql, new { userId }, _dbContext.CurrentTransaction);
            return res;
        }

        public async Task<bool> UpdateContinueDaysAsync(UserSignInInfo userSignInInfo)
        {
            string sql = @"
UPDATE UserSignInInfos SET ContinueDays = @ContinueDays, PreContinueDays = ContinueDays,LastSignDate = @LastSignDate,ModifyTime=@ModifyTime where Id=@Id And PreContinueDays = @PreContinueDays and ((CAST(LastSignDate as date) != CAST(@LastSignDate as date)) or LastSignDate is null)
";
            return (await _dbContext.Connection.ExecuteAsync(sql, new { Id = userSignInInfo.Id, ContinueDays = userSignInInfo.ContinueDays, LastSignDate = userSignInInfo.LastSignDate, PreContinueDays = userSignInInfo.PreContinueDays , ModifyTime = userSignInInfo.ModifyTime },_dbContext.CurrentTransaction)) > 0;

        }
    }
}
