using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enum;
using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public class CityCategoryRepository : ICityCategoryRepository
    {
        readonly LocalQueryDB _queryDB;

        public CityCategoryRepository(LocalQueryDB queryDB)
        {
            _queryDB = queryDB;
        }

        public async Task<Category> GetCategory(long id)
        {
            var sql = "select * from Category where id=@id";
            var r = await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<Category>(sql, new { id });
            return r;
        }

        public async Task<IEnumerable<CityDto>> GetCitys()
        {
            var sql = "select City as id,CityName as name,IsOpen,ShortName from CityInfo where IsValid=1 and IsOpen=1 order by City";
            var ls = await _queryDB.SlaveConnection.QueryAsync<CityDto>(sql, new { });
            return ls;
        }

        public async Task<IEnumerable<Category>> GetCategories(long city, long parentCategoryId)
        {
            var sql = @"
                select c.* 
                from CityCategory city 
                left join Category c on city.CategoryId=c.id
                where c.IsValid=1 and city.city=@city and c.parentid=@parentCategoryId
                order by c.sort
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<Category>(sql, new { city, parentCategoryId });
            return ls;
        }

        public async Task<IEnumerable<Category>> GetCategories(long city, string parentPath, bool includeSelf = false)
        {
            var sql = $@"
                select c.* 
                from CityCategory city 
                left join Category c on city.CategoryId=c.id
                where c.IsValid=1 and city.city=@city and left(c.path,len(@parentPath))=@parentPath {(!includeSelf ? "and c.path<>@parentPath" : "")}
                order by c.parentid,c.sort
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<Category>(sql, new { city, parentPath });
            return ls;
        }

        public async Task<IEnumerable<CategoryChildDto>> GetChildrenWithChildren(long city, long parentId, bool? canFindSchool = null)
        {
            var categories = await GetCategories(city, "/" + parentId.ToString() + "/", includeSelf: false);

            return categories
                .Where(s => s.Parentid == parentId)
                .Where(s => canFindSchool == null || s.CanFindSchool == canFindSchool)
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

        public async Task<IEnumerable<CategoryChildDto>> GetDepth1Or2Categories(long city)
        {
            var sql = @"
                select c.* 
                from CityCategory city 
                inner join Category c on city.CategoryId=c.id
                where c.IsValid=1 and city.city=@city and c.Depth in (1,2)
                order by c.sort
            ";
            var categories = await _queryDB.SlaveConnection.QueryAsync<Category>(sql, new { city });

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


        public async Task<Category> GetSchoolCategory(long parentId)
        {
            var sql = $"select * from Category where CanFindSchool = 1 and charindex('/{parentId}/', path)>0 ";
            return await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<Category>(sql, new { });
        }
    }
}