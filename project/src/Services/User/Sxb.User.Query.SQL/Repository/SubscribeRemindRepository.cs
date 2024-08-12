using Dapper;
using Sxb.User.Query.SQL.DB;
using Sxb.User.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.User.Query.SQL.Repository
{
    public class SubscribeRemindRepository : ISubscribeRemindRepository
    {
        SchoolUserDB _userDB;
        public SubscribeRemindRepository(SchoolUserDB schoolUserDB)
        {
            _userDB = schoolUserDB;
        }

        public async Task<IEnumerable<Guid>> GetUserIdsAsync(string groupCode, Guid subjectId, int pageIndex, int pageSize)
        {
            var sql = @"
Select Distinct UserId from SubscribeRemind 
WHERE
    IsValid = 1
    and IsEnable = 1
    and SubjectId = @subjectId 
    and GroupCode = @groupCode
Order By UserId
offset (@pageIndex-1)*@pageSize rows
fetch next @pageSize rows only
";
            return await _userDB.SlaveConnection.QueryAsync<Guid>(sql, new { groupCode, subjectId, pageIndex, pageSize });
        }


        public async Task<bool> ExistsAndSubscribeFwhAsync(string groupCode, Guid subjectId, Guid userId)
        {
            var sql = @"
Select top 1 SR.Id 
from
	SubscribeRemind SR
	inner join openid_weixin OW ON OW.userID = SR.UserId AND OW.AppName = 'fwh' AND OW.Valid = 1
WHERE
    SR.IsValid = 1
    and SR.SubjectId = @subjectId 
    and SR.GroupCode = @groupCode
    and SR.UserId = @userId
";
            var id = await _userDB.SlaveConnection.QueryFirstOrDefaultAsync<Guid?>(sql, new { groupCode, subjectId, userId });
            return id != null;
        }
    }
}
