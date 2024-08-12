using Sxb.School.Common.Enum;
using System.ComponentModel.DataAnnotations;

namespace Sxb.School.API.RequestContact.School
{
    public class PostCorrectionMsgRequest
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        [Required]
        public string Mobile { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// 验证码类型
        /// </summary>
        [Required]
        public string CodeType { get; set; }
        /// <summary>
        /// 身份类型
        /// </summary>
        [Required]
        public IdentityType IdentityType { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        public string Content { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [Required]
        public string Nickname { get; set; }
    }
}
