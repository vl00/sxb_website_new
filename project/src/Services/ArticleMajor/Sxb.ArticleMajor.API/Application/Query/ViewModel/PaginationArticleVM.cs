using Sxb.ArticleMajor.Common;
using System.Collections.Generic;

namespace Sxb.ArticleMajor.API.Application.Query.ViewModel
{
    /// <summary>
    /// 分页model
    /// </summary>
    public class PaginationArticleVM<T> : PaginationRespDto<T>
    {
        /// <summary>
        /// 查询类型下的子类型
        /// </summary>
        public List<ArticleCategoryVM> SubCategories { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public List<ArticleCategoryVM> LinkedParents { get; set; }
    }
}
