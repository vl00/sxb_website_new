using Sxb.Settlement.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace Sxb.Settlement.API.Infrastucture.Repositories
{
    public class UserRepository : IUserRepository
    {
        string _conStr;
        public UserRepository(string connectionString)
        {
            _conStr = connectionString;
        }

        public async Task<User> FindAsync(Guid id)
        {
            string sql = @"
 SELECT 
 userInfo.id,
 userInfo.nationCode,
 userInfo.mobile,
 userInfo.nickname
 FROM userInfo
 where id=@id";
            using (IDbConnection con = new SqlConnection(_conStr))
            {
                var user = await con.QueryFirstOrDefaultAsync<User>(sql,new { id });
                return user;
            }

        }
    }
}
