using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class UserGzWxDto
    {
        public Guid UserId { get; set; }        

        public bool? IsRealUser { get; set; }

        /// <summary>是否关注公众号</summary>
        public bool HasGzWxGzh { get; set; }
        /// <summary>
        /// 未关注公众号时返回二维码
        /// </summary>
        public string GzWxQrcode { get; set; }

        /// <summary>是否已加企业微信客服</summary>
        public bool HasJoinWxEnt { get; set; }
        /// <summary>
        /// 未加企业微信客服时返回二维码
        /// </summary>
        public string JoinWxEntQrcode { get; set; }
    }

    
}
