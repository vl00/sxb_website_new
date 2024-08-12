using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.QueryDto;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.ArticleMajor.API.Utils
{
    public class AppSettingsData
    {
        public List<CityDto> Cities { get; set; }

        public List<ProvinceDto> Provinces { get; set; }

        /// <summary>
        /// 高考网使用省份作为区分, 这里使用省会, 不用改用代码
        /// </summary>
        public List<CityDto> ImitationProvinces => Provinces.Select(s => CityDto.ParseProvince(s)).ToList();

        public Dictionary<ArticlePlatform, string> ArticleSites { get; set; } = new Dictionary<ArticlePlatform, string>();
    }
}
