using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Queries.UserInfo
{
    public class UserInfoQueries: IUserInfoQueries
    {
        private readonly string _connectionStr;
        public UserInfoQueries(string connectionString)
        {
            _connectionStr = connectionString;
        }

        public async Task<bool> GetSubscribeStatus(Guid id)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"SELECT count(1) FROM iSchoolUser.dbo.openid_weixin 
WHERE appName = 'fwh'  and valid = 1 and userID = @id
";
            var c = await con.ExecuteScalarAsync<int>(sql, new { id });
            return c > 0;
        }

        public async Task<string> GetOpenId(Guid id)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"SELECT openID FROM iSchoolUser.dbo.openid_weixin 
WHERE appName = 'fwh'  and valid = 1 and userID = @id
";
            var openId = await con.ExecuteScalarAsync<string>(sql, new { id });
            return openId;
        }
        public async Task<UserInfo> GetUserInfo(Guid id)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"
            SELECT id, nickname FROM iSchoolUser.dbo.userInfo
             WHERE id = @id";
            var userinfo = await con.QueryFirstOrDefaultAsync<UserInfo>(sql, new { id = id });
            return userinfo;


        }
    }
}
