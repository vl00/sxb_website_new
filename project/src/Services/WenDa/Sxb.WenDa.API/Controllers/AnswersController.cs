using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Application.Commands;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Filters;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IQuestionQuery _questionQuery;
        private readonly IAnswerQuery _answerQuery;
        private readonly IEasyRedisClient _easyRedisClient;

        public AnswersController(IQuestionQuery questionQuery, IAnswerQuery answerQuery, IEasyRedisClient easyRedisClient,
            IMediator mediator)
        {
            _mediator = mediator;
            _questionQuery = questionQuery;
            _answerQuery = answerQuery;
            _easyRedisClient = easyRedisClient;
        }

        /// <summary>
        /// 问题详情 (不含回答列表)
        /// </summary>
        /// <param name="questionId">问题id(长短都行)</param>
        /// <param name="answerId">
        /// 分享过来的回答id(长短都行)<br/>
        /// 普通情况进入问题详情可以不传此参数
        /// </param>
        /// <returns></returns>
        [HttpGet("q/detail")]
        [ProducesResponseType(typeof(APIResult<QuestionDetailVm>), 200)]
        public async Task<ResponseResult> GetQuestionDetail(string questionId, string answerId = null)
        {
            var r = await _questionQuery.GetQuestionDetail(questionId, answerId, UserId);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 回答列表分页
        /// </summary>
        /// <param name="questionId">问题id(长短都行)</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderby">
        /// 排序 1=(默认)点赞多到少 2=时间近到远
        /// </param>
        /// <returns></returns>
        [HttpGet("pagelist")]
        [ProducesResponseType(typeof(APIResult<Page<QaAnswerItemDto>>), 200)]
        public async Task<ResponseResult> GetAnswersPageList(string questionId, int pageIndex = 1, int pageSize = 20, int orderby = 1)
        {
            var qid = Guid.TryParse(questionId, out var _qid) ? _qid : default;
            var qno = qid == default ? UrlShortIdUtil.Base322Long(questionId) : default;
            if (qno != default)
            {
                qid = await _questionQuery.GetQuestionIdByNo(qno);
            }
            var r = await _answerQuery.GetAnswersPageList(qid, pageIndex, pageSize, (AnswersListOrderByEnum)orderby, UserId);
            return ResponseResult.Success(r);
        }


        /// <summary>
        /// 发回答
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("add")]
        [CheckBindMobile, CheckBindWeixin]
        [ResponseResultExceptionToResult]
        [ProducesResponseType(typeof(APIResult<AddAnswerCommandResult>), 200)]
        public async Task<ResponseResult> AddAnswerAsync(AddAnswerCommand cmd)
        {
            cmd.UserId = UserId;

            var lck1f = new StecRedisLock1Factory1(_easyRedisClient);
            var k = string.Format(CacheKeys.Wenda_lck_question, cmd.QuestionId);
            await using var lck1 = await lck1f.LockAsync(new Lock1Option(k, 60 * 2));
            if (!lck1.IsAvailable) throw new ResponseResultException("系统繁忙", Errcodes.Wenda_GetLck1Failed);

            var r = await _mediator.Send(cmd);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 加载邀请人列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("q/InviteUserList")]
        [ResponseResultExceptionToResult]
        [ProducesResponseType(typeof(APIResult<GetQuestionInviteUserLsQueryResult>), 200)]
        public async Task<ResponseResult> GetQuesInviteUserList([FromQuery] GetQuestionInviteUserLsQuery query)
        {
            var r = await _answerQuery.GetQuesInviteUserList(query);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 邀请别人来回答问题
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("q/InviteUserToAnswer")]
        [ResponseResultExceptionToResult]
        [ProducesResponseType(typeof(APIResult<InviteUserToAnswerQuestionCommandResult>), 200)]
        public async Task<ResponseResult> InviteUserToAnswerQuestion(InviteUserToAnswerQuestionCommand cmd)
        {
            cmd.UserId = UserId;

            var lck1f = new StecRedisLock1Factory1(_easyRedisClient);
            var k = string.Format(CacheKeys.Wenda_lck_InviteUserToAnswerQuestion, cmd.QuestionId, cmd.ToUserId);
            await using var lck1 = await lck1f.LockAsync(new Lock1Option(k, 60 * 2));
            if (!lck1.IsAvailable) throw new ResponseResultException("系统繁忙", Errcodes.Wenda_GetLck1Failed);

            var r = await _mediator.Send(cmd);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 加载回答用于编辑
        /// </summary>
        /// <param name="id">回答id</param>
        /// <returns></returns>
        [HttpGet("load")]
        [ProducesResponseType(typeof(APIResult<LoadAnswerForSaveDto>), 200)]
        public async Task<ResponseResult> LoadAnswerForSave(Guid id)
        {
            var r = await _answerQuery.GetAnswerByIdForSave(id);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 编辑回答
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("edit")]
        [CheckBindMobile, CheckBindWeixin]
        [ResponseResultExceptionToResult]
        [ProducesResponseType(typeof(APIResult<EditAnswerCommandResult>), 200)]
        public async Task<ResponseResult> EditAnswerAsync(EditAnswerCommand cmd)
        {
            cmd.UserId = UserId;

            var lck1f = new StecRedisLock1Factory1(_easyRedisClient);
            var k = string.Format(CacheKeys.Wenda_lck_answer, cmd.AnswerId);
            await using var lck1 = await lck1f.LockAsync(new Lock1Option(k).SetExpExpectBySec());
            if (!lck1.IsAvailable) throw new ResponseResultException("系统繁忙", Errcodes.Wenda_GetLck1Failed);

            var r = await _mediator.Send(cmd);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 删除回答
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("del")]
        [ResponseResultExceptionToResult]
        [ProducesResponseType(typeof(APIResult<DelAnswerCommandResult>), 200)]
        public async Task<ResponseResult> DelAnswerAsync(DelAnswerCommand cmd)
        {
            cmd.UserId = UserId;

            var lck1f = new StecRedisLock1Factory1(_easyRedisClient);
            var k = string.Format(CacheKeys.Wenda_lck_answer, cmd.AnswerId);
            await using var lck1 = await lck1f.LockAsync(new Lock1Option(k).SetExpExpectBySec());
            if (!lck1.IsAvailable) throw new ResponseResultException("系统繁忙", Errcodes.Wenda_GetLck1Failed);

            var r = await _mediator.Send(cmd);
            return ResponseResult.Success(r);
        }

    }
}
