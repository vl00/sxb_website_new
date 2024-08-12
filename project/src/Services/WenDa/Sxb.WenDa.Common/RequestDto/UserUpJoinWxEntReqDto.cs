using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.RequestDto
{
    public class UserUpJoinWxEntReqDto
    {
        /// <summary>UnionId</summary>
        public string UnionId { get; set; }

        //public string AttachData { get; set; }

        /// <summary>
        /// 是否已加企业微信客服
        /// 貌似接口就是加了企业微信后才被call的
        /// </summary>
        public bool HasJoinWxEnt { get; set; } = true;
    }

    //public class UserUpJoinWxEntReqDto_AttachData
    //{
    //    /// <summary>用户id</summary>
    //    public Guid UserId { get; set; }
    //    /// <summary>UnionId</summary>
    //    public string UnionId { get; set; }
    //}
}
