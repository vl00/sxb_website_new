using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.WenDa.API.Application.Commands;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Filters;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ICommentQuery _commentQuery;

        public CommentController(ICommentQuery commentQuery,
            IMediator mediator, IEasyRedisClient easyRedisClient)
        {
            _mediator = mediator;
            _easyRedisClient = easyRedisClient;
            _commentQuery = commentQuery;
        }

        /// <summary>
        /// 发评论
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>        
        [Authorize]
        [HttpPost("add")]
        [CheckBindMobile, CheckBindWeixin]
        [ProducesResponseType(typeof(APIResult<AddQaCommentCommandResult>), 200)]
        public async Task<ResponseResult> AddQaComment(AddQaCommentCommand cmd)
        {
            cmd.UserId = UserId;            
            var r = await _mediator.Send(cmd);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 评论列表分页 (1级和2级都用此接口)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("pagelist")]
        [ProducesResponseType(typeof(APIResult<GetCommentsPageListQryResDto>), 200)]
        public async Task<ResponseResult> GetCommentsPageList(GetCommentsPageListQuery query)
        {
            query.UserId = UserId;
            var r = await _commentQuery.GetCommentsPageList(query);
            return ResponseResult.Success(r);
        }
    }
}
