using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Framework.AspNetCoreHelper.ResponseModel
{
    /// <summary>
    /// 分页
    /// </summary>
    [Serializable]    
    public class Page<T>
    {
        public Page()
        {
        }

        public Page(IEnumerable<T> data, int total)
        {
            Data = data;
            Total = total;
        }

        /// <summary>
        /// 当前页码下数据
        /// </summary>
        public IEnumerable<T> Data { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int Total { get; set; }

        public Page<NewT> ChangeData<NewT>(IEnumerable<NewT> data)
        {
            return data.ToPage(Total);
        }
    }


    public static partial class PageExtensions
    {
        public static Page<T> ToPage<T>(this IEnumerable<T> data, int total)
        {
            return new Page<T>() { Data = data, Total = total };
        }
    }
}
