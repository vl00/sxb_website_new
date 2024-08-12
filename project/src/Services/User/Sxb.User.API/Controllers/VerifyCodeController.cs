using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.User.API.RequestContract.VerifyCode;
using Sxb.User.Common.DTO;
using System.Threading.Tasks;

namespace Sxb.User.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class VerifyCodeController : ControllerBase
    {
        readonly IEasyRedisClient _easyRedisClient;
        public VerifyCodeController(IEasyRedisClient easyRedisClient)
        {
            _easyRedisClient = easyRedisClient;
        }

        [HttpPost]
        public async Task<ResponseResult> VerifyRndCode(VerifyRndCodeRequest request)
        {
            var result = ResponseResult.Failed("param error");
            if (request == null || string.IsNullOrWhiteSpace(request.Code)) return result;

            if (!CommonHelper.isMobile(request.Mobile))
            {
                return ResponseResult.Failed("请输入有效的手机号码", new
                {
                    errorstatus = 1,
                    errorDescription = "请输入有效的手机号码"
                });
            }


            RndCodeDTO CodeCache = await _easyRedisClient.GetAsync<RndCodeDTO>("login:RNDCode-" + request.NationCode + request.Mobile + "-" + request.CodeType);

            string insideRndCodeKey = "login:RndCode-Inside";
            string insideRndCode = _easyRedisClient.GetStringAsync(insideRndCodeKey).Result;

            if (!string.IsNullOrWhiteSpace(insideRndCode) && request.Code == insideRndCode)
            {
                return ResponseResult.Success("验证成功");
            }
            if (CodeCache != null && CodeCache.Code == request.Code && CodeCache.CodeType == request.CodeType)
            {
                await _easyRedisClient.RemoveAsync("login:RNDCode-" + request.NationCode + request.Mobile + "-" + request.CodeType);
                return ResponseResult.Success("验证成功");
            }


            result = ResponseResult.Failed("验证失败", new
            {
                errorstatus = 1,
                errorDescription = "请输入正确的验证码"
            });
            return result;
        }
    }
}
