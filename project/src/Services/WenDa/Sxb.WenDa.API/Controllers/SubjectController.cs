using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.API.Application.Commands;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Filters;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.API.Controllers
{
    /// <summary>
    /// 专栏
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISubjectQuery _subjectQuery;

        public SubjectController(ISubjectQuery subjectQuery,
            IMediator mediator)
        {
            _mediator = mediator;
            _subjectQuery = subjectQuery;
        }

        /// <summary>
        /// 专栏详情 (不含列表分页)
        /// </summary>
        /// <param name="subjectId">专栏id(长短都行)</param>
        /// <param name="city">城市编码</param>
        /// <returns></returns>
        [HttpGet(nameof(Detail))]
        [ResponseResultExceptionToResult]
        [ProducesResponseType(typeof(APIResult<SubjectDetailVm>), 200)]
        public async Task<ResponseResult> Detail(string subjectId, long? city = null)
        {
            city = city <= 0 ? null : city;
            var r = await _subjectQuery.GetSubjectDetail(subjectId, city, UserId);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 专栏里问答列表分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("q/pagelist")]
        [ResponseResultExceptionToResult]
        [ProducesResponseType(typeof(APIResult<Page<QaQuestionListItemDto>>), 200)]
        public async Task<ResponseResult> GetQuestionsBySubject([FromQuery] GetQuestionsPageListBySubjectQuery query)
        {
            query.UserId = UserId;
            var r = await _subjectQuery.GetQuestionsBySubject(query);
            return ResponseResult.Success(r);
        }
    }
}
