using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.API.Application.Commands;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.API.Controllers
{
    /// <summary>
    /// 用户关注领域
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserCategoryAttentionsController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        private readonly IUserCategoryAttentionQuery _userCategoryAttentionQuery;

        public UserCategoryAttentionsController(IMediator mediator, IUserCategoryAttentionQuery userCategoryAttentionQuery)
        {
            _mediator = mediator;
            _userCategoryAttentionQuery = userCategoryAttentionQuery;
        }

        /// <summary>
        /// 设置关注领域
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>        
        [HttpPost("update")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        public async Task<ResponseResult> SetAttentionAsync([FromBody] UserCategoryAttentionRequestDto request)
        {
            return await _mediator.Send(new UserCategoryAttentionCommand()
            {
                UserId = UserId,
                CategoryIds = request.CategoryIds,
                CreateTime = DateTime.Now
            });
        }

        /// <summary>
        /// 获取我的关注领域
        /// </summary>
        /// <returns></returns>        
        [HttpGet("mine")]
        [Authorize]
        [ProducesResponseType(typeof(AttentionCategoryDto), 200)]
        public async Task<ResponseResult> GetAttentions()
        {
            var data = await _userCategoryAttentionQuery.GetUserCategories(UserId);
            return ResponseResult.Success(data);
        }


    }
}
