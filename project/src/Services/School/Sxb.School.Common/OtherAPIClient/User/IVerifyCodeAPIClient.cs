using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.User
{
    public interface IVerifyCodeAPIClient
    {
        /// <summary>
        /// 检验验证码
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="codeType">验证码类型</param>
        /// <param name="code">验证码</param>
        /// <param name="nationCode">国家代号</param>
        /// <returns></returns>
        Task<bool> VerifyRndCodeAsync(string mobile, string codeType, string code, string nationCode = "86");
    }
}
