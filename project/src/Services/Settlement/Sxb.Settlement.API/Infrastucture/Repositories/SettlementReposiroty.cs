using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
namespace Sxb.Settlement.API.Model
{
    public class SettlementReposiroty : ISettlementReposiroty
    {
        string _conStr;
        public SettlementReposiroty(string connectionString)
        {
            _conStr = connectionString;
        }

        public async Task AddAsync(Settlement settlement)
        {
            string sql = @"INSERT INTO [dbo].[Settlement]
           ([Id]
           ,[OrderNum]
           ,[Amount]
           ,[WxAppId]
           ,[WxOpenId]
           ,[BusinessSceneCode]
           ,[StatusCallBackUrl]
           ,[Status]
           ,[Remark]
           ,[CreateTime])
     VALUES
           (@Id
           ,@OrderNum
           ,@Amount
           ,@WxAppId
           ,@WxOpenId
           ,@BusinessSceneCode
           ,@StatusCallBackUrl
           ,@Status
           ,@Remark
           ,@CreateTime)";
            using (IDbConnection con = new SqlConnection(_conStr))
            {
                int flag = await con.ExecuteAsync(sql, new
                {
                    Id = settlement.Id,
                    OrderNum = settlement.OrderNum,
                    Amount = settlement.Amount,
                    WxAppId = settlement.WxAppId,
                    WxOpenId = settlement.WxOpenId,
                    BusinessSceneCode = settlement.BusinessSceneCode,
                    StatusCallBackUrl = settlement.StatusCallBackUrl,
                    Status = settlement.Status,
                    Remark = settlement.Remark,
                    CreateTime = DateTime.Now
                });
            }
        }

        public async Task<Settlement> FindByOrderNumAsync(string orderNum)
        {
            string sql = @"SELECT * FROM Settlement WHERE OrderNum=@orderNum";
            using (IDbConnection con = new SqlConnection(_conStr))
            {
                return await con.QueryFirstOrDefaultAsync<Settlement>(sql, new { orderNum });
            }
        }

        public async Task<bool> UpdateStatusByAsync(string orderNum, SettlementStatus settlementStatus, string remark)
        {
            string sql = @"UPDATE [iSchoolSettlement].[dbo].[Settlement] SET [STATUS] = @settlementStatus,REMARK = @remark WHERE OrderNum=@orderNum";
            using (IDbConnection con = new SqlConnection(_conStr))
            {
                int flag = await  con.ExecuteAsync(sql, new { orderNum , settlementStatus, remark });
                return flag > 0;
            }
        }
    }
}
