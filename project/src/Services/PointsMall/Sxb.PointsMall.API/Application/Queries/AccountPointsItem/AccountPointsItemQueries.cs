using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using Sxb.PointsMall.API.Application.Models;

namespace Sxb.PointsMall.API.Application.Queries.AccountPointsItem
{
    public class AccountPointsItemQueries : IAccountPointsItemQueries
    {
        private readonly string _connectionStr;

        public AccountPointsItemQueries(string connectionStr)
        {
            _connectionStr = connectionStr;
        }

        public async Task<AccountPointsDetails> GetAccountPointsDetails(GetAccountPointsDetailsFilter filter)
        {
            using var con = new SqlConnection(_connectionStr);
            DynamicParameters parameters = new DynamicParameters(new { userId = filter.UserId, stime = filter.STime, etime = filter.ETime, offset = filter.Offset, limit = filter.Limit });
            string sql = @$"SELECT 
	  Id
      ,[UserId]
      ,[Points]
      ,[OriginType]
      ,[OriginId]
      ,[IsFreeze]
      ,[Remark]
      ,[ModifyTime]
  FROM [iSchoolPointsMall].[dbo].[AccountPointsItems]
  WHERE
  ModifyTime  BETWEEN @stime AND @etime
  AND  PARENTID IS NULL
  AND UserId = @userId
  {(filter.Direction? "AND Points >= 0" : "AND Points < 0")}
  ORDER BY ModifyTime DESC
  OFFSET @offset ROWS
  FETCH NEXT @limit ROWS ONLY;

  SELECT 
	ISNULL(SUM(Points),0)  ConsumeTotal
  FROM [iSchoolPointsMall].[dbo].[AccountPointsItems]
  WHERE
  ModifyTime   BETWEEN  @stime AND @etime
  AND Points < 0
  AND PARENTID IS NULL
  AND UserId = @userId;
  
  SELECT 
	ISNULL(SUM(Points),0) AwardTotal
  FROM [iSchoolPointsMall].[dbo].[AccountPointsItems]
  WHERE
  ModifyTime   BETWEEN  @stime AND @etime
  AND Points >= 0
  AND PARENTID IS NULL
  AND UserId = @userId;

";
            using var grid = await con.QueryMultipleAsync(sql, parameters);
            var accountPointsItems = await grid.ReadAsync<AccountPointsItem>();
            long consumeTotal = await grid.ReadFirstAsync<long>();
            long awardTotal = await grid.ReadFirstAsync<long>();
            return new AccountPointsDetails()
            {
                AwardTotal = awardTotal,
                ConsumeTotal = consumeTotal,
                Items = accountPointsItems.ToList()
            };


        }
    }
}
