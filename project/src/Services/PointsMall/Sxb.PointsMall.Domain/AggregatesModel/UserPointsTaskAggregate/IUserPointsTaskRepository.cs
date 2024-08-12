using Sxb.PointsMall.Domain.AggregatesModel.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate
{
    public interface IUserPointsTaskRepository : IRepository<UserPointsTask>
    {
        Task<bool> AddAsync(UserPointsTask userPointsTask);

        Task<bool> UpdateAsync(UserPointsTask userPointsTask, params string[] fields);

        Task<UserPointsTask> FindFromAsync(Guid uesrId);
        Task<PointsTask> FindTaskFromAsync(int taskId);
        Task<UserPointsTask> FindFromAsync(Guid userId, int taskId, UserPointsTaskStatus? status, DateTime? taskDate);
        Task<(int todayTimes, int totalTimes)> FindCountAsync(Guid userId, int taskId, DateTime? taskDate);
    }
}
