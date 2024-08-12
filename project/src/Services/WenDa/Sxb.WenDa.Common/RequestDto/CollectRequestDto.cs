using Sxb.WenDa.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.RequestDto
{
    public class CollectRequestDto
    {
        /// <summary>
        /// 收藏类型  收藏类型  1=问题 2=专栏
        /// </summary>
        public UserCollectType Type { get; set; }

        /// <summary>
        /// 收藏人
        /// </summary>
        //public Guid UserId { get; set; }

        /// <summary>
        /// 收藏对象id
        /// </summary>
        public Guid DataId { get; set; }

        /// <summary>
        /// 是否收藏
        /// </summary>
        public bool IsValid { get; set; }
    }
}
