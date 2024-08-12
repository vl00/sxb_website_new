using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Queries.DgAyAddCustomerUser
{
    public class DgAyAddCustomerUserQueries : IDgAyAddCustomerUserQueries
    {
        private readonly string _connectionStr;
        public DgAyAddCustomerUserQueries(string connectionString)
        {
            _connectionStr = connectionString;
        }
        public async Task<bool> GetStatus(Guid userId)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"SELECT count(1)
  FROM [iSchoolData].[dbo].[DgAyAddCustomerUser]
  WHERE UserId = @userId";
            return (await con.ExecuteScalarAsync<int>(sql, new { userId })) > 0;
        }
    }
}
