using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.OtherAPIClient.Marketing.Model
{
    public class UserWxOpenInfo
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string UserPhone { get; set; }

        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 微信openid
        /// </summary>
        public string OpenId { get; set; }

    }
}
