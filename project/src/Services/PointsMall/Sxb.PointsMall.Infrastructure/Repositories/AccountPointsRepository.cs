
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
    public class AccountPointsRepository : IAccountPointsRepository
    {
        public IUnitOfWork UnitOfWork => _dbContext;

        PointsMallDbContext _dbContext;

        public AccountPointsRepository(PointsMallDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(AccountPoints accountPoints)
        {
            string sql = @"INSERT INTO [dbo].[AccountPoints]
           ([Id]
           ,[UserId]
           ,[Points]
           ,[FreezePoints]
           ,[ModifyTime])
SELECT 
           @Id
           ,@UserId
           ,@Points
           ,@FreezePoints
           ,@ModifyTime 
WHERE  NOT EXISTS(SELECT 1 FROM  [dbo].[AccountPoints] WHERE UserId = @UserId)"; 
            await _dbContext.Connection.ExecuteAsync(sql, accountPoints, _dbContext.CurrentTransaction);
        }

        public async Task<bool> UpdateAsync(AccountPoints accountPoints, params string[] fields)
        {
            if (fields == null || !fields.Any()) throw new ArgumentNullException($"{nameof(fields)} is empty.");
            string upTemplate = "Update {0} Set {1} Where Id = @Id";
            IEnumerable<string> sets = fields.Select(f => $"[{f}]= @{f}");
            string sql = string.Format(upTemplate, "AccountPoints", string.Join(",", sets));
            return (await _dbContext.Connection.ExecuteAsync(sql, accountPoints, _dbContext.CurrentTransaction)) > 0;
        }

        public async Task<AccountPoints> GetAsync(Guid id)
        {
            string sql = @"SELECT * FROM AccountPoints WHERE Id=@id";
            return await _dbContext.Connection.QueryFirstOrDefaultAsync<AccountPoints>(sql, new { id }, _dbContext.CurrentTransaction);
        }

        public async Task<AccountPoints> FindFromAsync(Guid userId)
        {
            string sql = @"SELECT id,userId,points,freezePoints,modifyTime FROM AccountPoints WHERE UserId=@userId";
            var res = await _dbContext.Connection.QueryFirstOrDefaultAsync<AccountPoints>(sql, new { userId }, _dbContext.CurrentTransaction);
            return res;

        }

     
    }
}
