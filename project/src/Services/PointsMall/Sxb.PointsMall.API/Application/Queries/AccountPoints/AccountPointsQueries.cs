using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Sxb.PointsMall.API.Application.Queries.AccountPoints
{
    public class AccountPointsQueries : IAccountPointsQueries
    {
        private readonly string _connectionStr;

        public AccountPointsQueries(string connectionStr)
        {
            _connectionStr = connectionStr;
        }

        public async Task<AccountPoints> GetAccountPoints(Guid userId)
        {

            using (IDbConnection con = new SqlConnection(_connectionStr))
            {
                string sql = @"
SELECT Id, UserId,Points,FreezePoints FROM AccountPoints
WHERE UserId = @userId";

                var res = await con.QueryFirstOrDefaultAsync<AccountPoints>(sql, new { userId });
                if (res == null)
                    return new AccountPoints() { UserId = userId, FreezePoints = 0, Points = 0 };
                return res;
            }

        }

        public async Task<IEnumerable<Guid>> GetPointsOverZeroUserIds()
        {
            using (IDbConnection con = new SqlConnection(_connectionStr))
            {
                string sql = @"
SELECT UserId FROM AccountPoints where Points  >0";

                var res = await con.QueryAsync(sql);
                return res.Select(s => (Guid)s.UserId);
            }

        }
    }
}
