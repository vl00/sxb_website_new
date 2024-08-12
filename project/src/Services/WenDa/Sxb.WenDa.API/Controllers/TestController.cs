using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Application.Commands;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Filters;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>        
        [HttpGet]
        [ResponseResultExceptionToResult]
        [ProducesResponseType(typeof(APIResult<QaQuestionListItemDto>), 200)]
        public async Task<ResponseResult> Tx1()
        {
            await default(ValueTask);
            return ResponseResult.Success(new QaQuestionListItemDto { Answer = new() });
        }

        /// <summary>
        /// no and id_s
        /// </summary>
        /// <param name="id"></param>
        /// <param name="i">
        /// 1= id_s to no <br/>
        /// 2= no to id_s <br/>
        /// </param>
        /// <returns></returns>
        [HttpGet("no")]
        [ResponseResultExceptionToResult]
        [ProducesResponseType(typeof(APIResult<object>), 200)]
        public async Task<ResponseResult> Tx_no(string id, int i)
        {
            if (i == 1)
            {
                var no = UrlShortIdUtil.Base322Long(id);
                return ResponseResult.Success(new { src = id, id_s = id, no });
            }
            if (i == 2)
            {
                if (!long.TryParse(id, out var no)) throw new ResponseResultException("id is not long", 201);
                var id_s = UrlShortIdUtil.Long2Base32(no);
                return ResponseResult.Success(new { src = id, id_s, no });
            }
            await default(ValueTask);
            return null;
        }

        [HttpPost(nameof(Tx_GetQaItemDtos))]
        [ProducesResponseType(typeof(APIResult<QaItemDto[]>), 200)]
        public async Task<ResponseResult> Tx_GetQaItemDtos(string[] idstrs, 
            [FromServices] IQuestionQuery _questionQuery)
        {
            static IEnumerable<(Guid, long)> Get_Ids_Nos(IEnumerable<string> ids)
            {
                foreach (var str_id in ids)
                {
                    var id = Guid.TryParse(str_id, out var _id) ? _id : default;
                    var no = id == default ? UrlShortIdUtil.Base322Long(str_id) : default;
                    yield return (id, no);
                }
            }

            var ls = Get_Ids_Nos(idstrs);
            var ids = ls.Where(_ => _.Item1 != default).Select(_ => _.Item1).ToList();
            var nos = ls.Where(_ => _.Item2 != default).Select(_ => _.Item2).ToList();

            var rr = await _questionQuery.GetQaItemDtos<QaItemDto>(ids, nos);
            return ResponseResult.Success(rr);
        }

        /// <summary>
        /// real user
        /// </summary>
        [HttpPost(nameof(GetUserGzWxDto))]
        [ProducesResponseType(typeof(APIResult<UserGzWxDto>), 200)]
        public async Task<ResponseResult> GetUserGzWxDto(Guid userId,
            [FromServices] IUserQuery _userQuery)
        {
            var r = await _userQuery.GetUserGzWxDto(userId);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 判断用户是否关注公众号+是否已加企业微信客服
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="_userQuery"></param>
        /// <returns></returns>
        [HttpGet("/users/gzwx")]
        [ProducesResponseType(typeof(APIResult<UserGzWxDto>), 200)]
        public async Task<ResponseResult> GetUserGzWx(Guid userId, 
            [FromServices] IUserQuery _userQuery)
        {
            var r = await _userQuery.GetUserGzWxDto(userId);
            return ResponseResult.Success(r);
        }
    }
}
