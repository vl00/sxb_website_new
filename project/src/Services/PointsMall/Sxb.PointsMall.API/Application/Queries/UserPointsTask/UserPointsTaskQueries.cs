using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;

namespace Sxb.PointsMall.API.Application.Queries.UserPointsTask
{
    public class UserPointsTaskQueries : IUserPointsTaskQueries
    {

        private readonly string _connectionStr;

        public UserPointsTaskQueries(string connectionStr)
        {
            _connectionStr = connectionStr;
        }

        public async Task<IEnumerable<PointsTasksOfUser>> GetScrollPointsTasksOfUser(Guid userId, int offset = 0, int limit = 3)
        {
            using (var con = new SqlConnection(_connectionStr))
            {
                DateTime now = DateTime.Now;
                string sql = @"
IF(EXISTS( SELECT 1 FROM tempdb..SYSOBJECTS WHERE id = OBJECT_ID('tempdb..#POINTTASKOFUSER')))
BEGIN 
DROP TABLE #POINTTASKOFUSER
END
SELECT 
  pt.[Id]
 ,pt.[Name]
 ,pt.[Type]
 ,pt.[TimesPoints]
 ,pt.[MaxTimesEveryDay]
 ,pt.[Desc]
 ,ISNULL(pt.[MaxTimes],999999999) [MaxTimes]
 ,ISNULL((SELECT Count(1) FROM [iSchoolPointsMall].[dbo].[UserPointsTasks] WHERE IsValid = 1 AND Status = 1 AND UserId = @userId AND PointsTaskId = pt.Id AND CAST(@now AS date) = CAST(CreateTime AS date)),0) [WaitGrantTimesToday]
 ,ISNULL((SELECT Count(1) FROM [iSchoolPointsMall].[dbo].[UserPointsTasks] WHERE IsValid = 1 AND Status = 1 AND UserId = @userId AND PointsTaskId = pt.Id),0) [WaitGrantTimes]
 ,ISNULL((SELECT Count(1) FROM [iSchoolPointsMall].[dbo].[UserPointsTasks] WHERE IsValid = 1 AND Status != 0 AND UserId = @userId AND PointsTaskId = pt.Id AND CAST(@now AS date) = CAST(CreateTime AS date)),0) [FinishTimesToday]
 ,ISNULL((SELECT Count(1) FROM [iSchoolPointsMall].[dbo].[UserPointsTasks] WHERE IsValid = 1 AND Status != 0 AND UserId = @userId AND PointsTaskId = pt.Id ),0) [FinishTimes] --完成总次数
 ,sort
 INTO #POINTTASKOFUSER
FROM [iSchoolPointsMall].[dbo].[PointsTasks] pt
WHERE IsEnable = 1

SELECT
  ptu.*
 ,1  as batch
  FROM #POINTTASKOFUSER ptu
  WHERE 
  ptu.MaxTimes > ptu.FinishTimes
  AND
  ptu.FinishTimesToday < ptu.MaxTimesEveryDay 
   UNION ALL
(SELECT 
  ptu.*
 ,2  as batch
 FROM #POINTTASKOFUSER ptu
   WHERE 
   ptu.MaxTimes  <= ptu.FinishTimes
  OR 
   ptu.FinishTimesToday >=  ptu.MaxTimesEveryDay 
)
order by batch ,[Type],sort 
offset @offset row fetch next @limit rows only
";
                return await con.QueryAsync<PointsTasksOfUser>(sql, new { userId, now, offset, limit });

            }
        }

        public async Task<IEnumerable<PointsTasksOfUser>> GetPointsTasksOfUser(Guid userId)
        {
            using (var con = new SqlConnection(_connectionStr))
            {
                DateTime now = DateTime.Now;
                string sql = @"SELECT pt.[Id]
      ,pt.[Name]
      ,pt.[Type]
      ,pt.[TimesPoints]
      ,pt.[MaxTimesEveryDay]
      ,pt.[Desc]
      ,ISNULL(pt.[MaxTimes],999999999) [MaxTimes]
      ,ISNULL((SELECT Count(1) FROM [iSchoolPointsMall].[dbo].[UserPointsTasks] WHERE IsValid = 1 AND Status = 1 AND UserId = @userId AND PointsTaskId = pt.Id AND CAST(@now AS date) = CAST(CreateTime AS date)),0) [WaitGrantTimesToday]
      ,ISNULL((SELECT Count(1) FROM [iSchoolPointsMall].[dbo].[UserPointsTasks] WHERE IsValid = 1 AND Status = 1 AND UserId = @userId AND PointsTaskId = pt.Id),0) [WaitGrantTimes]
      ,ISNULL((SELECT Count(1) FROM [iSchoolPointsMall].[dbo].[UserPointsTasks] WHERE IsValid = 1 AND Status != 0 AND UserId = @userId AND PointsTaskId = pt.Id AND CAST(@now AS date) = CAST(CreateTime AS date)),0) [FinishTimesToday]
      ,ISNULL((SELECT Count(1)FROM [iSchoolPointsMall].[dbo].[UserPointsTasks] WHERE IsValid = 1 AND Status != 0 AND UserId = @userId AND PointsTaskId = pt.Id ),0) [FinishTimes] --完成总次数
  FROM [iSchoolPointsMall].[dbo].[PointsTasks] pt
  WHERE IsEnable = 1
  Order By pt.Sort
";
                return await con.QueryAsync<PointsTasksOfUser>(sql, new { userId, now });

            }
        }

        public async Task<UserTaskStatusViewModel> GetCurrentAsync(Guid userId, long taskId)
        {
            using (var con = new SqlConnection(_connectionStr))
            {
                DateTime now = DateTime.Now;
                string taskSql = @"
SELECT pt.[Id] as TaskId, pt.[MaxTimesEveryDay], pt.[MaxTimes]
FROM [iSchoolPointsMall].[dbo].[PointsTasks] pt
Where pt.Id = @taskId and IsEnable = 1
;
";
                var vm = await con.QueryFirstOrDefaultAsync<UserTaskStatusViewModel>(taskSql, new { userId, taskId, now });
                if (vm != null)
                {
                    vm.FinishTimesToday = await con.QueryFirstOrDefaultAsync<byte>(GetTodayTimesSqlAsync(), new { userId, taskId, now });
                    vm.FinishTimes = await con.QueryFirstOrDefaultAsync<byte>(GetTotalTimesSqlAsync(), new { userId, taskId, now });
                }
                return vm;
            }
        }


        public string GetTodayTimesSqlAsync()
        {
            return @"
SELECT 
    count(1)
FROM [iSchoolPointsMall].[dbo].[UserPointsTasks] 
WHERE
    IsValid = 1
    AND UserId = @userId 
    AND PointsTaskId = @taskId
    AND Status != 0
    AND CAST(@now AS date) = CAST(EndFinishTime AS date)
;
";
        }

        public string GetTotalTimesSqlAsync()
        {
            return @"
SELECT 
    count(1)
FROM [iSchoolPointsMall].[dbo].[UserPointsTasks] 
WHERE
    IsValid = 1
    AND UserId = @userId 
    AND PointsTaskId = @taskId
    AND Status != 0
;
";
        }
    }
}
