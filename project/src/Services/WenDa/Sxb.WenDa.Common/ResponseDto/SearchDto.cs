using Sxb.WenDa.Common.Enums;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class SearchDto
    {
        public static SearchDto Create<T>(RefTable type, T data)
        {
            return new SearchDto
            {
                Type = type,
                Data = data
            };
        }

        /// <summary>
        /// 类型  1 问题  3 专栏
        /// </summary>
        public RefTable Type { get; set; }

        /// <summary>
        /// 数据 
        /// 1.问题,参考问答列表item 
        /// 3.专栏,参考专栏页面item
        /// </summary>
        public object Data { get; set; }
    }
}
