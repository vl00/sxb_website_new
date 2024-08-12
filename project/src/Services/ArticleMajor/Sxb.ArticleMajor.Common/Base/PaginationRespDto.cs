using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Common
{
    /// <summary>
    /// 分页model
    /// </summary>
    public class PaginationRespDto
    {
        public static PaginationRespDto<TEntity> Build<TEntity>(IEnumerable<TEntity> data, long total)
        {
            return new PaginationRespDto<TEntity>()
            {
                Data = data,
                Total = total
            };
        }
    }

    /// <summary>
    /// 分页model
    /// </summary>
    public class PaginationRespDto<T> : PaginationRespDto
    {
        /// <summary>
        /// 分页数据
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// 总记录数量
        /// </summary>
        public long Total { get; set; }

        public static PaginationRespDto<T> Build()
        {
            return Build(new List<T>(), 0);
        }
    }

    /// <summary>
    /// 分页model
    /// </summary>
    public class PaginationExtRespDto<T> : PaginationRespDto<T>
    {
        /// <summary>
        /// 附加数据
        /// </summary>
        public object Extra { get; set; }
        public PaginationExtRespDto(IEnumerable<T> data, long total, object extra)
        {
            Data = data;
            Total = total;
            Extra = extra;
        }
    }
}
