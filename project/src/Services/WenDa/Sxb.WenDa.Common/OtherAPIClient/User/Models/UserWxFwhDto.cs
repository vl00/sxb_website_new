using System;

namespace Sxb.WenDa.Common.OtherAPIClient.User.Models
{
    /// <summary>
    /// UserWxFwhDto
    /// </summary>
    public class UserWxFwhDto
    {
        /// <summary>
        /// openid
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsSubscribe { get; set; }
    }
}
