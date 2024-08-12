using System.ComponentModel.DataAnnotations;

namespace Sxb.User.API.RequestContract.VerifyCode
{
    public class VerifyRndCodeRequest
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        [Required]
        public string Mobile { get; set; }
        /// <summary>
        /// 国家区号
        /// </summary>
        public string NationCode { get; set; } = "86";
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
    }
}
