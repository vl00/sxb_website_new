using Sxb.ArticleMajor.Common;
using Sxb.ArticleMajor.Common.Enum;

namespace Sxb.ArticleMajor.API.Application.Query.RequestModel
{
    public class ArticlesPaginationReqDto  : PaginationReqDto
    {
        /// <summary>
        /// 子站  1幼儿教育 2小学教育 3中学教育 4中职网 5高中教育 6素质教育 7国际教育
        /// </summary>
        public ArticlePlatform Platform { get; set; }

        /// <summary>
        /// 类型短名称
        /// </summary>
        public string CategoryShortName{ get; set; }

        /// <summary>
        /// 城市短名称
        /// </summary>
        public string CityShortName { get; set; }

    }
}
