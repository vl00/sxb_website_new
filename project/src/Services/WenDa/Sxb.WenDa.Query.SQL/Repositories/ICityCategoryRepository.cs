using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface ICityCategoryRepository
    {
        Task<Category> GetCategory(long id);
        Task<IEnumerable<CityDto>> GetCitys();
        /// <summary>查城市分类</summary>
        Task<IEnumerable<Category>> GetCategories(long city, long parentCategoryId);
        /// <summary>查城市分类的所有标签</summary>
        Task<IEnumerable<Category>> GetCategories(long city, string parentPath, bool includeSelf = false);
        Task<Category> GetSchoolCategory(long parentId);
        Task<IEnumerable<CategoryChildDto>> GetChildrenWithChildren(long city, long parentPath, bool? canFindSchool = null);
        Task<IEnumerable<CategoryChildDto>> GetDepth1Or2Categories(long city);
    }
}
