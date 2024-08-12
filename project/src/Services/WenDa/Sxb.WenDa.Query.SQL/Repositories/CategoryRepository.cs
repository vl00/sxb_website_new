using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        readonly LocalQueryDB _queryDB;

        public CategoryRepository(LocalQueryDB queryDB)
        {
            _queryDB = queryDB;
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _queryDB.SlaveConnection
                    .QuerySet<Category>()
                    .Where(s => s.IsValid == true)
                    .ToListAsync()
                    ;
        }

        public async Task<IEnumerable<CategoryChildDto>> GetPlatformChildren()
        {
            var categories = await _queryDB.SlaveConnection
                    .QuerySet<Category>()
                    .Where(s => s.IsValid == true)
                    .Where(s => s.Depth == 1 || s.Depth == 2)
                    .ToListAsync()
                    ;
            return categories.Where(s => s.Depth == 1)
                .Select(s =>
                    new CategoryChildDto(
                        s.Id,
                        s.Name,
                        categories
                            .Where(child => child.Parentid == s.Id)
                            .Select(child => new CategoryChildDto(child.Id, child.Name, null))
                            .ToList()
                   )
                );
        }


        public async Task<IEnumerable<Category>> GetQuesCategoryIdsByAttentionCategoryIds(IEnumerable<long> attentionCategoryIds)
        {
            if (attentionCategoryIds?.Any() != true) return Enumerable.Empty<Category>();
            var sql = $@"
                select c.* from Category c where c.IsValid=1 and c.type=1
                and ( {string.Join(" or ", attentionCategoryIds.Select(_ => $"charindex('/{_}/',c.path)>0"))} )
            ";
            var ls = (await _queryDB.SlaveConnection.QueryAsync<Category>(sql)).AsList();           
            return ls;
        }
    }
}