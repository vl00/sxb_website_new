using System;

namespace Sxb.User.Common.DTO
{
    /// <summary>
    /// unionid_weixin dto
    /// </summary>
    public class UserWxUnionIdDto
    {
        public string UnionId { get; set; }
        /// <summary>用户id</summary>
        public Guid UserId { get; set; }
        /// <summary>用户名</summary>
        public string NickName { get; set; }
        /// <summary>是否绑定</summary>
        public bool Valid { get; set; }
    }
}
