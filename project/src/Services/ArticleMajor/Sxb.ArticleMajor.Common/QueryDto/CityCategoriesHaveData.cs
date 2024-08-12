using System.Collections.Generic;
using System.Linq;

namespace Sxb.ArticleMajor.Common.QueryDto
{

    /// <summary>
    /// 城市中有数据的分类
    /// </summary>
    public class CityCategoriesHaveData
    {
        public int CityId { get; set; }

        public List<int> Categories { get; set; }


        public static List<int> GetCategoryIds(IEnumerable<CityCategoriesHaveData> data, int cityId)
        {
            if (data== null)
            {
                return new List<int>();
            }
            return data.Where(s => s.CityId == 0 || s.CityId == cityId).SelectMany(s => s.Categories).ToList();
        }


        public static bool HasCategoryId(IEnumerable<CityCategoriesHaveData> data, int cityId, int categoryId)
        {
            return data != null
                && data
                    .Where(s => s.CityId == 0 || s.CityId == cityId)
                    .Any(s => s.Categories.Contains(categoryId));
        }
    }
}
