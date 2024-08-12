using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Queries.DgAyOrder
{
    public class DgAyOrderQueries : IDgAyOrderQueries
    {
        private readonly string _connectionStr;
        public DgAyOrderQueries(string connectionString)
        {
            _connectionStr = connectionString;
        }

        public async Task<bool> ExistsFreeOrder(Guid userId)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"SELECT 1 FROM DgAyOrder
WHERE
UserId = @userId
AND
[Status]  =  2001 AND PayWay = 1";
            return (await con.ExecuteScalarAsync<int>(sql, new { userId })) > 0;
        }


        public async Task<DgAyOrderStateSummary> GetDgAyOrderStateAsync(Guid orderId)
        {
            using IDbConnection con = new SqlConnection(_connectionStr);
            string sql = @"SELECT Id,UserId , [Status] State FROM DgAyOrder WHERE Id = @id";
            var stateSummary = await con.QueryFirstOrDefaultAsync<DgAyOrderStateSummary>(sql, new { id = orderId });
            return stateSummary;
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
      ,[ExpireTime]
      ,[CreateTime]
      ,[UpdateTime]
  FROM [dbo].[DgAyOrder] WHERE Id = @id";
            var orderDetail = await con.QueryFirstOrDefaultAsync<OrderDetail>(sql, new { id = id });
            if (orderDetail == null)
                throw new KeyNotFoundException();
            string queryProductInfoSql = @"SELECT 
productId id, productName [name],productDesc [desc],[ProductType] [type]
,[Number]
,[UnitPrice]
,[OriginUnitPrice]
FROM  DgAyOrderDetail where orderid = @oid";
            orderDetail.ProductInfos = (await con.QueryAsync<ProductInfo>(queryProductInfoSql, new { oid = id })).ToList();
            return orderDetail;
        }
    }
}
