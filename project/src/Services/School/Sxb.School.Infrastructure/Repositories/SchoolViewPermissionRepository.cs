using Sxb.School.Domain.AggregateModels.ViewOrder;
using Sxb.School.Domain.AggregateModels.ViewPermission;
using Sxb.School.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Infrastructure.Repositories
{
    using Dapper;
    using System.Linq;

    public class SchoolViewPermissionRepository : ISchoolViewPermissionRepository
    {

        SchoolDataDbContext _dbContext;

        public SchoolViewPermissionRepository(SchoolDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IUnitOfWork UnitOfWork => _dbContext;

        public async Task AddAsync(SchoolViewPermission schoolViewPermission)
        {
            string sql = @"INSERT INTO [dbo].[SchoolViewPermission]
           ([Id]
           ,[UserId]
           ,[ExtId]
           ,[IsDel]
           ,[IsValid]
           ,[CreateTime]
           ,[UpdateTime])
     VALUES
           (@Id
           ,@UserId
           ,@ExtId
           ,0
           ,@IsValid
           ,@CreateTime
           ,@UpdateTime)";
            await _dbContext.Connection.ExecuteAsync(sql, schoolViewPermission, _dbContext.CurrentTransaction);
        }

        public async Task<SchoolViewPermission> FindFromUserAndExtAsync(Guid userId, Guid extId)
        {
            string sql = @"SELECT  [Id]
      ,[UserId]
      ,[ExtId]
      ,[IsDel]
      ,[IsValid]
      ,[CreateTime]
      ,[UpdateTime]
  FROM [iSchoolData].[dbo].[SchoolViewPermission]
  WHERE UserId = @UserId AND ExtId = @ExtId AND IsDel = 0";
            return await _dbContext.Connection.QueryFirstOrDefaultAsync<SchoolViewPermission>(sql, new { UserId = userId, ExtId = extId }, _dbContext.CurrentTransaction);

        }

        public async Task<SchoolViewPermission> GetAsync(Guid id)
        {
            string sql = @"SELECT  [Id]
      ,[UserId]
      ,[ExtId]
      ,[IsDel]
      ,[IsValid]
      ,[CreateTime]
      ,[UpdateTime]
  FROM [iSchoolData].[dbo].[SchoolViewPermission]
  WHERE Id = @id AND IsDel = 0";
            return await _dbContext.Connection.QueryFirstOrDefaultAsync<SchoolViewPermission>(sql, new { id }, _dbContext.CurrentTransaction);
        }

        public async Task<bool> UpdateAsync(SchoolViewPermission schoolViewPermission, params string[] fields)
        {
            if (fields == null || fields.Any() == false) return false;
            string sql = string.Format(@"UPDATE [dbo].[SchoolViewPermission]  SET {0}  WHERE id = @id and isDel = 0", string.Join(",", fields.Select(f => $"[{f}] = @{f}")));
            return (await _dbContext.Connection.ExecuteAsync(sql, schoolViewPermission, _dbContext.CurrentTransaction)) > 0;
        }
    }
}
