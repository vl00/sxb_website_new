
using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using Sxb.PointsMall.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace Sxb.PointsMall.Infrastructure.Repositories
{
    public class AccountPointsItemRepository : IAccountPointsItemRepository
    {
        public IUnitOfWork UnitOfWork => _dbContext;

        PointsMallDbContext _dbContext;

        public AccountPointsItemRepository(PointsMallDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task AddAsync(AccountPointsItem accountPointsItem)
        {
            string sql = @"INSERT INTO [dbo].[AccountPointsItems]
           ([Id]
           ,[UserId]
           ,[Points]
           ,[OriginType]
           ,[OriginId]
           ,[IsFreeze]
           ,[Remark]
           ,[ModifyTime]
           ,[State]
           ,[CreateTime]
           ,[ParentId])
     VALUES
           (@Id
           ,@UserId
           ,@Points
           ,@OriginType
           ,@OriginId
           ,@IsFreeze
           ,@Remark
           ,@ModifyTime
           ,@State
           ,@ModifyTime
           ,@ParentId)";
            await _dbContext.Connection.ExecuteAsync(sql, accountPointsItem, _dbContext.CurrentTransaction);

        }




        public async Task<bool> UpdateAsync(AccountPointsItem  accountPointsItem, params string[] fields)
        {
            if (fields == null || !fields.Any()) throw new ArgumentNullException($"{nameof(fields)} is empty.");
            string upTemplate = "Update {0} Set {1} Where Id = @Id  and isDel =0 ";
            IEnumerable<string> sets = fields.Select(f => $"[{f}]= @{f}");
            string sql = string.Format(upTemplate, "AccountPointsItems", string.Join(",", sets));
            return (await _dbContext.Connection.ExecuteAsync(sql, accountPointsItem, _dbContext.CurrentTransaction)) > 0;
        }

        public async Task<AccountPointsItem> GetAsync(Guid id)
        {
            string sql = @"SELECT 
       [Id]
      ,[UserId]
      ,[Points]
      ,[OriginType]
      ,[OriginId]
      ,[IsFreeze]
      ,[Remark]
      ,[ModifyTime]
      ,[State]
      ,[ParentId]
  FROM [iSchoolPointsMall].[dbo].[AccountPointsItems]
  WHERE Id = @id and   isDel =0";
            return await _dbContext.Connection.QueryFirstOrDefaultAsync<AccountPointsItem>(sql, new { id }, _dbContext.CurrentTransaction);
        }


        public async Task<bool> DeleteAsync(AccountPointsItem accountPoints)
        {
            string sql = @"  update [iSchoolPointsMall].[dbo].[AccountPointsItems] set isDel=1  where id=@id  and isDel =0";
            return (await _dbContext.Connection.ExecuteAsync(sql,new { id = accountPoints.Id},_dbContext.CurrentTransaction) > 0);
        }
    }
}
