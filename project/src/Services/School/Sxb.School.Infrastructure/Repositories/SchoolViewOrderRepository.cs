using Sxb.School.Domain.AggregateModels.ViewOrder;
using Sxb.School.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Infrastructure.Repositories
{
    using Dapper;
    using System.Linq;

    public class SchoolViewOrderRepository: ISchoolViewOrderRepository
    {

        SchoolDataDbContext _dbContext;

        public SchoolViewOrderRepository(SchoolDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IUnitOfWork UnitOfWork => _dbContext;

        public async Task AddAsync(SchoolViewOrder schoolViewOrder)
        {
            string sql = @"INSERT INTO [dbo].[SchoolViewOrder]
           ([Id]
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
           ,[UpdateTime])
     VALUES
           (@Id
           ,@Num
           ,@UserId
           ,@PayWay
           ,@Amount
           ,@Payment
           ,@Status
           ,@Remark
           ,@GoodsInfo
           ,@ExpireTime
           ,@CreateTime
           ,@UpdateTime)";
           await  _dbContext.Connection.ExecuteAsync(sql, schoolViewOrder, _dbContext.CurrentTransaction);


        }

        public async Task<SchoolViewOrder> GetAsync(Guid id)
        {
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
          return   await _dbContext.Connection.QueryFirstOrDefaultAsync<SchoolViewOrder>(sql, new { id }, _dbContext.CurrentTransaction);
        }

        public async Task<bool> UpdateAsync(SchoolViewOrder schoolViewOrder, params string[] fields)
        {
            if (fields == null || fields.Any() == false) return false;
            string sql = string.Format(@"UPDATE [dbo].[SchoolViewOrder]  SET {0}  WHERE id = @id", string.Join(",", fields.Select(f => $"[{f}] = @{f}")));
            return (await _dbContext.Connection.ExecuteAsync(sql, schoolViewOrder, _dbContext.CurrentTransaction)) > 0;

        }
    }
}
