using Sxb.School.Domain.AggregateModels.ViewOrder;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Queries.SchoolViewOrder
{
    using Dapper;
    using System.Collections.Generic;
    using System.Data;

    public class SchoolViewOrderQueries : ISchoolViewOrderQueries
    {

        private readonly string _connectionStr;
        public SchoolViewOrderQueries(string connectionString)
        {
            _connectionStr = connectionString;
        }



        public async Task<bool> ExistsPermissionAsync(Guid userId, Guid eid)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"SELECT 1
  FROM [iSchoolData].[dbo].[SchoolViewPermission]
WHERE
UserId = @userId 
AND ExtId = @eid
AND IsDel = 0 
AND IsValid=1";
            return (await con.ExecuteScalarAsync<int>(sql, new { userId , eid })) > 0;
        }

        public async Task<bool> ExistsFreeOrder(Guid userId)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"SELECT 1 FROM SchoolViewOrder
WHERE
UserId = @userId
AND
[Status]  =  2001 AND PayWay = 1";
             return  (await con.ExecuteScalarAsync<int>(sql, new { userId })) > 0;
        }



        public async Task<OrderDetail> GetOrderDetailAsync(Guid id)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"SELECT [Id]
      ,[Num]
      ,[UserId]
      ,[PayWay]
      ,[Amount]
      ,[Payment]
      ,[Status]
      ,[Remark]
      ,[GoodsInfo]
      ,[ExpireTime]
      ,[CreateTime]
      ,[UpdateTime]
  FROM [dbo].[SchoolViewOrder] WHERE Id = @id";
            var orderDetail = await con.QueryFirstOrDefaultAsync<OrderDetail>(sql, new { id = id });
            if (orderDetail == null)
                throw new KeyNotFoundException();
            return orderDetail;
        }

        public async Task<ViewOrderStateSummary> GetViewOrderStateAsync(Guid orderId)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"SELECT Id,UserId , [Status] State FROM SchoolViewOrder WHERE Id = @id";
            var  stateSummary = await con.QueryFirstOrDefaultAsync<ViewOrderStateSummary>(sql, new { id = orderId });
            return stateSummary;
        }

        public async Task<SchoolInfo> GetSchoolInfo(Guid eid)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"  SELECT  OnlineSchool.id,OnlineSchoolExtension.id eid,(OnlineSchool.[name] + OnlineSchoolExtension.[name]) [name]  FROM OnlineSchoolExtension
  JOIN OnlineSchool ON OnlineSchool.id = OnlineSchoolExtension.sid
  WHERE OnlineSchoolExtension.id = @eid";
            var schoolInfo = await con.QueryFirstOrDefaultAsync<SchoolInfo>(sql, new { eid = eid });
            return schoolInfo;
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

        public async Task<bool> IsALevelSchool(Guid eid)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = "SELECT 1 FROM SchoolViewALevel WHERE ID = @eid";
            int? flag = await con.ExecuteScalarAsync<int?>(sql, new { eid });
            return flag > 0;
        }
    }
}
