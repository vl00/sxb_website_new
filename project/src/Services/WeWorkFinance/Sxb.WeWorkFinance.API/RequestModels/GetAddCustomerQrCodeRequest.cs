using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.RequestModels
{
    public class GetAddCustomerQrCodeRequest
    {
        /// <summary>
        /// 顾问UnionId
        /// </summary>
        public string AdviserId { get; set; }

        /// <summary>
        /// 顾问用户Id
        /// </summary>
        public Guid AdviserUserId { get; set; }

        /// <summary>
        /// 发展人UnionId
        /// </summary>
        public string InviterId { get; set; }

        /// <summary>
        /// 发展人OpenId
        /// </summary>
        public string InviterOpenId { get; set; }
    }
}
