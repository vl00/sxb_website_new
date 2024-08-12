using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.RequestModels
{
    public class MPSendMsgRequest
    {
        /// <summary>
        /// 发展人UnionId
        /// </summary>
        public string UnionId { get; set; }

        /// <summary>
        /// 发展人OpenId
        /// </summary>
        public string OpenId { get; set; }

        public string Text { get; set; }
    }
}
