using Sxb.Settlement.API.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Sxb.Settlement.API.Application.Queries
{
    public class UserIDCardQueries : IUserIDCardQueries
    {
        string _constr;
        public UserIDCardQueries(string connectionString)
        {
            _constr = connectionString;
        }

        public async Task<bool> ExistsIDCard(IDCard card)
        {
            using (var con = new SqlConnection(_constr))
            {
                string sql = @"select count(1) from UserIDCard where UserId = @UserId and Number = @Number";

                var count = await con.ExecuteScalarAsync<int>(sql, new { Number = card.Number, UserId = card.UserId });
                return count > 0;
            }
        }

        public async Task<bool> ExistsOthersHasSignAsync(IDCard card)
        {
            using (var con = new SqlConnection(_constr))
            {
                string sql = @"SELECT COUNT(1) FROM UserIDCard where
Number= @Number and IsSign =1 And UserId != @UserId";

              var count =  await con.ExecuteScalarAsync<int>(sql, new { Number = card.Number, IsSign = card.IsSign , UserId = card.UserId});
                return count > 0;
            }

        }

        public async Task<IDCard> GetFirstIdCard(Guid userId)
        {
            using (var con = new SqlConnection(_constr))
            {
                string sql = @"SELECT top 1 * FROM UserIDCard
WHERE  UserId = @userId
ORDER BY UpdateTime DESC,CreateTime DESC";

                return await con.QueryFirstOrDefaultAsync<IDCard>(sql, new { userId = userId });
            }
        }

        public async Task<IDCard> GetFirstSignIdCard(Guid userId)
        {
            using (var con = new SqlConnection(_constr))
            {
                string sql = @"SELECT top 1 * FROM UserIDCard
WHERE IsSign =1 AND UserId = @userId
ORDER BY UpdateTime DESC,CreateTime DESC";

                return await con.QueryFirstOrDefaultAsync<IDCard>(sql, new { userId = userId });
            }

        }

        public async Task<IDCard> GetFirstUnSignIdCard(Guid userId)
        {
            using (var con = new SqlConnection(_constr))
            {
                string sql = @"SELECT top 1 * FROM UserIDCard
WHERE IsSign =0 AND UserId = @userId
ORDER BY UpdateTime DESC,CreateTime DESC";

                return await con.QueryFirstOrDefaultAsync<IDCard>(sql, new { userId = userId });
            }
        }


        public async Task<IDCard> GetFirstMaybeSignIdCard(Guid userId)
        {
            using (var con = new SqlConnection(_constr))
            {
                string sql = @"SELECT top 1 * from UserIDCard
WHERE 
UserId = @userId
ORDER BY IsSign DESC,UpdateTime DESC,  CreateTime DESC";

                return await con.QueryFirstOrDefaultAsync<IDCard>(sql, new { userId = userId });
            }
        }


        public async Task<IDCard> GetFirstMaybeUnSignIdCard(Guid userId)
        {
            using (var con = new SqlConnection(_constr))
            {
                string sql = @"SELECT top 1 * from UserIDCard
WHERE 
UserId = @userId
ORDER BY IsSign ASC,UpdateTime DESC,  CreateTime DESC";

                return await con.QueryFirstOrDefaultAsync<IDCard>(sql, new { userId = userId });
            }
        }

        public async Task<int> GetSignCount(Guid userId)
        {
            using (var con = new SqlConnection(_constr))
            {
                string sql = @"SELECT 
COUNT(1) 
FROM UserIDCard
WHERE UserId = @userId
AND IsSign = 1";

                return await con.ExecuteScalarAsync<int>(sql, new { userId = userId });
            }
        }
    }
}
