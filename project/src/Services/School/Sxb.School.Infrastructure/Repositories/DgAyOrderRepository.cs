using Dapper;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using Sxb.School.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Infrastructure.Repositories
{
    public class DgAyOrderRepository : IDgAyOrderRepository
    {
        public IUnitOfWork UnitOfWork => _dbContext;

        SchoolDataDbContext _dbContext;

        public DgAyOrderRepository(SchoolDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(DgAyOrder order)
        {
            string sql = @"INSERT INTO [dbo].[DgAyOrder]
           ([Id]
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
           ,[Termtyp])
     VALUES
           (@Id
           ,@Num
           ,@UserId
           ,@PayWay
           ,@Amount
           ,@Payment
           ,@Status
           ,@Remark
           ,@ExpireTime
           ,@CreateTime
           ,@UpdateTime
           ,@Termtyp)";
            await _dbContext.Connection.ExecuteAsync(sql, order,_dbContext.CurrentTransaction);
            foreach (var detail in order.OrderDetails)
            {
                string sql2 = @"INSERT INTO [dbo].[DgAyOrderDetail]
           ([Id]
           ,[OrderId]
           ,[ProductId]
           ,[ProductType]
           ,[ProductName]
           ,[ProductDesc]
           ,[Number]
           ,[UnitPrice]
           ,[OriginUnitPrice]
           ,[CreateTime])
     VALUES
           (@Id
           ,@OrderId
           ,@ProductId
           ,@ProductType
           ,@ProductName
           ,@ProductDesc
           ,@Number
           ,@UnitPrice
           ,@OriginUnitPrice
           ,@CreateTime)";
                await _dbContext.Connection.ExecuteAsync(sql2,detail,_dbContext.CurrentTransaction);

            }

        }

        public async Task<DgAyOrder> GetAsync(Guid id)
        {
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
            var order = await _dbContext.Connection.QueryFirstOrDefaultAsync<DgAyOrder>(sql, new { id }, _dbContext.CurrentTransaction);


            return order;
        }

        public async Task<bool> UpdateAsync(DgAyOrder order, params string[] fields)
        {
            if (fields == null || fields.Any() == false) return false;
            string sql = string.Format(@"UPDATE [dbo].[DgAyOrder] SET {0}  WHERE id = @id", string.Join(",", fields.Select(f => $"[{f}] = @{f}")));
            return (await _dbContext.Connection.ExecuteAsync(sql, order, _dbContext.CurrentTransaction)) > 0;
        }

        public async Task<IEnumerable<DgAyOrderDetail>> GetDgAyOrderDetailsAsync(Guid id)
        {
            string sql = @"SELECT [Id]
      ,[OrderId]
      ,[ProductId]
      ,[ProductType]
      ,[ProductName]
      ,[ProductDesc]
      ,[Number]
      ,[UnitPrice]
      ,[OriginUnitPrice]
      ,[CreateTime]
  FROM [dbo].[DgAyOrderDetail]
  WHERE OrderId = @orderId";
            var orderdetails = await _dbContext.Connection.QueryAsync<DgAyOrderDetail>(sql, new { orderId = id }, _dbContext.CurrentTransaction);
            return orderdetails;


        }
    }
}
