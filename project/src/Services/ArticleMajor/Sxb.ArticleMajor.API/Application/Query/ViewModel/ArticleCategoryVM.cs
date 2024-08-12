using Sxb.ArticleMajor.Common.QueryDto;

namespace Sxb.ArticleMajor.API.Application.Query.ViewModel
{
    public class ArticleCategoryVM
    {
        public ArticleCategoryVM()
        {
        }

        public ArticleCategoryVM(string name, string shortName)
        {
            Name = name;
            ShortName = shortName;
        }

        /// <summary>
        /// 名称 e.g.中考备考
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 短名称 e.g.beikao
        /// </summary>
        public string ShortName { get; set; }

        public static explicit operator ArticleCategoryVM(CategoryQueryDto queryDto)
        {
            return new ArticleCategoryVM()
            {
                Name = queryDto.Name,
                ShortName = queryDto.ShortName,
            };
        }
    }
}
