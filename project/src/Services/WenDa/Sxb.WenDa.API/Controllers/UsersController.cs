using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.Foundation;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.API.Application.Commands;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.Common.OtherAPIClient.WeChat;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.API.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserQuery _userQuery;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserQuery userQuery,
            IMediator mediator,
            ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _userQuery = userQuery;
            _logger = logger;
        }


        /// <summary>
        /// 点赞/取消点赞
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>        
        [HttpPost("like")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        public async Task<ResponseResult> LikeAsync([FromBody] LikeCommand request)
        {
            return await _mediator.Send(new LikeCommand()
            {
                Type = request.Type,
                UserId = UserId,
                DataId = request.DataId,
                IsValid = request.IsValid,
                CreateTime = DateTime.Now
            });
        }

        /// <summary>
        /// 判断用户是否关注公众号+是否已加企业微信客服
        /// </summary>
        /// <returns></returns>
        [HttpGet("gzwx")]
        [Authorize]
        [ProducesResponseType(typeof(APIResult<UserGzWxDto>), 200)]
        public async Task<ResponseResult> GetUserGzWx()
        {
            var userId = HttpContext.GetUserInfo()?.UserId ?? default;
            var r = await _userQuery.GetUserGzWxDto(userId);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// [后端] 加入企业微信de回调接口
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("up/joinwxent")]
        [ProducesResponseType(typeof(APIResult<UserUpJoinWxEntResDto>), 200)]
        public async Task<ResponseResult> UpUserJoinWxEnt(UserUpJoinWxEntReqDto req)
        {
            var r = await _userQuery.UpJoinWxEnt(req);
            return ResponseResult.Success(r);
        }



        /// <summary>
        /// 获取绑定微信的二维码
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("QrCode/BindWx")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseResult<string>), 200)]
        public async Task<ResponseResult> GetBindWxQrCodeAsync()
        {
            //_logger.LogInformation("GetBindWxQrCodeAsync:{0}", Request.Headers.ToList().Where(s=>s.Key != "Cookie").ToJson());
            var r = await _userQuery.GetWxQrCodeAsync(UserId, SubscibeSence.BindWx);
            return r.ToResponseResult();
        }


        /// <summary>
        /// 获取关注微信的二维码
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("QrCode/Subscribe")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseResult<string>), 200)]
        public async Task<ResponseResult> GetSubscribeWxQrCodeAsync()
        {
            var r = await _userQuery.GetWxQrCodeAsync(UserId, SubscibeSence.Subscribe);
            return r.ToResponseResult();
        }

        /// <summary>
        /// [后端] 回调绑定微信的二维码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("QrCode/BindWx/CallBack")]
        public async Task BindWxQrCodeCallBackAsync([FromBody] WPScanCallBackData data)
        {
            await _userQuery.BindWxQrCodeCallBackAsync(data);
        }

        /// <summary>
        /// [后端] 回调关注微信的二维码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("QrCode/Subscribe/CallBack")]
        public async Task SubscribeWxQrCodeCallBackAsync([FromBody] WPScanCallBackData data)
        {
            await _userQuery.SubscribeWxQrCodeCallBackAsync(data);
        }
    }
}
