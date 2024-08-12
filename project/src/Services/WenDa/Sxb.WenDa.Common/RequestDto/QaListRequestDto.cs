using Sxb.WenDa.Common.Enum;
using System.ComponentModel.DataAnnotations;

namespace Sxb.WenDa.Common.RequestDto
{
    public class QaListRequestDto
    {
        /// <summary>
        /// 平台
        /// </summary>
        public ArticlePlatform Platform { get; set; }

        /// <summary>
        /// 城市id
        /// </summary>
        public int City { get; set; } = 440100; 

        /// <summary>
        /// 分类id, 传0则查询全部数据, 即:热门推荐
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 分页数量
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
