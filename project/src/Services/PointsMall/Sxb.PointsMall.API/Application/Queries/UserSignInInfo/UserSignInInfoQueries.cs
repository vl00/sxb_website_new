using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
namespace Sxb.PointsMall.API.Application.Queries.UserSignInInfo
{
    public class UserSignInInfoQueries: IUserSignInInfoQueries
    {
        private readonly string _connectionStr;

        public UserSignInInfoQueries(string connectionStr)
        {
            _connectionStr = connectionStr;
        }

        public async Task<UserSignInInfo> GetUserSignInInfo(Guid userId)
        {
            using (var con = new SqlConnection(_connectionStr))
            {
                string sql = @"SELECT EnableNotify,ContinueDays,LastSignDate FROM UserSignInInfos WHERE UserId = @userId";
               var userSignInfo = await  con.QueryFirstOrDefaultAsync<UserSignInInfo>(sql,new { userId});
                if (userSignInfo == null)
                    return new UserSignInInfo() {  ContinueDays = 0, EnableNotify = false, LastSignDate = null};
                return userSignInfo;
            }
        }


        public async Task<IEnumerable<NotifyUserInfoViewModel>> GetNotifyUsers()
        {
            using (var con = new SqlConnection(_connectionStr))
            {
                string sql = @"SELECT UserId, ContinueDays, LastSignDate FROM UserSignInInfos WHERE EnableNotify = 1";
                return await con.QueryAsync<NotifyUserInfoViewModel>(sql, new {  });
            }
        }
    }
}
