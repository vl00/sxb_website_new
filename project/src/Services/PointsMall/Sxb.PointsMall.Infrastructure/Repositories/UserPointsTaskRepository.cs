using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using Sxb.PointsMall.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Sxb.PointsMall.Infrastructure.Repositories
{
    public class UserPointsTaskRepository : IUserPointsTaskRepository
    {
        public IUnitOfWork UnitOfWork => _dbContext;

        PointsMallDbContext _dbContext;

        public UserPointsTaskRepository(PointsMallDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddAsync(UserPointsTask userPointsTask)
        {
            string sql = @"
INSERT INTO [dbo].[UserPointsTasks]
(
    [Id]
    ,[UserId]
    ,[PointsTaskId]
    ,[GetPoints]
    ,[CreateTime]
    ,[EndFinishTime]
    ,[Remark]
)
SELECT 
    @Id
    ,@UserId
    ,@PointsTaskId
    ,@GetPoints
    ,@CreateTime
    ,@EndFinishTime
    ,@Remark
";
            return await _dbContext.Connection.ExecuteAsync(sql, userPointsTask, _dbContext.CurrentTransaction) > 0;
        }

        public async Task<bool> UpdateAsync(UserPointsTask userPointsTask, params string[] fields)
        {
            if (fields == null || !fields.Any()) throw new ArgumentNullException($"{nameof(fields)} is empty.");
            string upTemplate = "Update {0} Set {1} Where Id = @Id";
            IEnumerable<string> sets = fields.Select(f => $"[{f}]= @{f}");
            string sql = string.Format(upTemplate, "UserPointsTasks", string.Join(",", sets));
            return (await _dbContext.Connection.ExecuteAsync(sql, userPointsTask, _dbContext.CurrentTransaction)) > 0;

        }

        public async Task<UserPointsTask> FindFromAsync(Guid uesrId)
        {
            throw new NotImplementedException();
        }

        public async Task<UserPointsTask> FindFromAsync(Guid userId, int taskId, UserPointsTaskStatus? status, DateTime? taskDate)
        {
            string sql = @"
SELECT * 
FROM 
    UserPointsTasks upt 
Where 
    upt.IsValid = 1
    and upt.UserId = @userId
    and upt.PointsTaskId = @taskId
    and (@status is null or upt.Status = @status)
    and (@taskDate is null or 
            CAST(upt.CreateTime AS date) = CAST(@taskDate AS date))
Order by
    upt.CreateTime DESC
";


            var userPointsTask = await _dbContext.Connection.QueryFirstOrDefaultAsync<UserPointsTask>(sql
                , new { userId, taskId, status, taskDate }, _dbContext.CurrentTransaction);
            if (userPointsTask != null)
            {
                userPointsTask.PointsTask = await FindTaskFromAsync(taskId);
            }
            return userPointsTask;
        }

        public Task<PointsTask> FindTaskFromAsync(int taskId)
        {
            string sql = @" SELECT * FROM PointsTasks pt Where pt.Id = @taskId ";
            return _dbContext.Connection.QueryFirstOrDefaultAsync<PointsTask>(sql, new { taskId }, _dbContext.CurrentTransaction);
        }


        public async Task<(int todayTimes, int totalTimes)> FindCountAsync(Guid userId, int taskId, DateTime? taskDate)
        {
            taskDate ??= DateTime.Now;
            string sql = @"
SELECT count(1) as Total
FROM 
    UserPointsTasks upt 
Where 
    upt.IsValid = 1
    and upt.UserId = @userId
    and upt.PointsTaskId = @taskId
    and (@taskDate is null or 
            CAST(upt.CreateTime AS date) = CAST(@taskDate AS date))";

            var todayTimes = await _dbContext.Connection.QueryFirstOrDefaultAsync<int>(sql
                , new { userId, taskId, taskDate }, _dbContext.CurrentTransaction);

            DateTime? allTaskDate = null;
            var totalTimes = await _dbContext.Connection.QueryFirstOrDefaultAsync<int>(sql
                , new { userId, taskId, taskDate = allTaskDate }, _dbContext.CurrentTransaction);

            return (todayTimes, totalTimes);
        }

    }
}
