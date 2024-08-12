using Dapper;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Query.SQL.QueryDto;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public class InvitationRepository : IInvitationRepository
    {
        readonly LocalQueryDB _queryDB;

        public InvitationRepository(LocalQueryDB queryDB)
        {
            _queryDB = queryDB;
        }

        /// <summary>
        /// 有新被邀请的用户
        /// 定时（每天晚上8点检查前一天晚上8点到当晚8点是否有收到邀请，若收到，则推送该消息）
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<NotifyUserQueryDto>> GetInvitationToUserAsync(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var sql = $@"
SELECT
   I.ToUserId AS UserId, MIN(Q.No) as No
FROM Invitation I
	INNER JOIN Question Q ON Q.Id = I.QId AND Q.IsValid = 1
WHERE 
    I.IsValid = 1
    AND I.InviteTime >= @startTime 
    AND I.InviteTime < @endTime
GROUP BY I.ToUserId
ORDER BY I.ToUserId
offset (@pageIndex-1)*@pageSize rows 
FETCH next @pageSize rows only
";
            var param = new { startTime, endTime, pageIndex, pageSize };
            return await _queryDB.SlaveConnection.QueryAsync<NotifyUserQueryDto>(sql, param);
        }

    }
}
