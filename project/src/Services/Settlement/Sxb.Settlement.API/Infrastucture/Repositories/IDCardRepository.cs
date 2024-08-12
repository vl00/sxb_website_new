using Sxb.Settlement.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Sxb.Settlement.API.Infrastucture.Repositories
{
    public class IDCardRepository : IIDCardRepository
    {

        string _conStr;
        public IDCardRepository(string connectionString)
        {
            _conStr = connectionString;
        }

        public async Task<bool> AddAsync(IDCard IDCard)
        {
            string sql = @"
INSERT INTO [dbo].[UserIDCard]
           ([UserId]
           ,[Name]
           ,[Number]
           ,[IsSign]
           ,[CreateTime],[UpdateTime])
SELECT 
            @UserId
           ,@Name
           ,@Number
           ,@IsSign
           ,@CreateTime,@UpdateTime
WHERE NOT EXISTS(SELECT 1 FROM UserIDCard WHERE UserId = @UserId AND Number=@Number )
";
            using (IDbConnection con = new SqlConnection(_conStr))
            {
                var flag = await con.ExecuteAsync(sql, IDCard);
                return flag > 0;
            }
        }

        public async Task<bool> UpdateAsync(IDCard IDCard)
        {
            string sql = @"UPDATE [dbo].[UserIDCard] SET [Name] = @Name,[Number]=@Number,[IsSign] = @IsSign,[UpdateTime]=@UpdateTime WHERE [UserId]=@UserId And [Number]=@Number";
            using (IDbConnection con = new SqlConnection(_conStr))
            {
                var flag = await con.ExecuteAsync(sql, IDCard);
                return flag > 0;
            }
        }

        public async Task<IDCard> FindByUserIdAsync(Guid userId,string number)
        {
            string sql = @"SELECT
  [UserId]
  ,[Name]
  ,[Number]
  ,[IsSign]
  ,[CreateTime],[UpdateTime]
  FROM [iSchoolUser].[dbo].[UserIDCard]
  WHERE UserId = @userId And Number=@number
";

            using (IDbConnection con = new SqlConnection(_conStr))
            {
                var IDCard = await con.QueryFirstOrDefaultAsync<IDCard>(sql, new { userId, number });
                return IDCard;
            }
        }
    }
}
