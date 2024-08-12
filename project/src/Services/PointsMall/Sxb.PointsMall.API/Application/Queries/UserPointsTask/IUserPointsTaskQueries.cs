using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Queries.UserPointsTask
{
    public  interface IUserPointsTaskQueries
    {
        Task<UserTaskStatusViewModel> GetCurrentAsync(Guid userId, long taskId);
        Task<IEnumerable<PointsTasksOfUser>> GetPointsTasksOfUser(Guid userId);

        Task<IEnumerable<PointsTasksOfUser>> GetScrollPointsTasksOfUser(Guid userId,int offset = 0,int limit = 3);
    }
}
