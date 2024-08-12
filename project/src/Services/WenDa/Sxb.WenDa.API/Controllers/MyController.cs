using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.API.Application.Commands;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Filters;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.API.Controllers
{
    /// <summary>
    /// 个人中心-我的 页面
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MyController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISubjectQuery _subjectQuery;
        private readonly IQuestionQuery _questionQuery;
        private readonly IAnswerQuery _answerQuery;
        private readonly IMyQuery _myQuery;

        public MyController(IMyQuery myQuery,
            ISubjectQuery subjectQuery, IQuestionQuery questionQuery, IAnswerQuery answerQuery,
            IMediator mediator)
        {
            _mediator = mediator;
            _myQuery = myQuery;
            _subjectQuery = subjectQuery;
            _questionQuery = questionQuery;
            _answerQuery = answerQuery;
        }

        /// <summary>
        /// 我的-提问数+回答数+获赞数
        /// </summary>        
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(APIResult<MyWendaVm>), 200)]
        public async Task<ResponseResult> GetMyWenda()
        {
            var r = await _myQuery.GetMyWenda(UserId);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 待回答-推荐回答 首次进入先用此接口
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("QuestionsToBeAnswered")]
        [ProducesResponseType(typeof(APIResult<QuestionsToBeAnsweredVm>), 200)]
        public async Task<ResponseResult> GetQuestionsToBeAnswered(int pageIndex, int pageSize)
        {
            var r = await _questionQuery.GetMyQuestionsToBeAnswered(UserId, pageIndex, pageSize);
            return ResponseResult.Success(r);
        }
        /// <summary>
        /// 待回答-邀请回答
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("QuestionsToBeAnswered/invited")]
        [ProducesResponseType(typeof(APIResult<Page<ToBeAnsweredQuestionListItemDto>>), 200)]
        public async Task<ResponseResult> GetQuestionsToBeAnswered_InvitedMe(int pageIndex, int pageSize)
        {
            var r = await _questionQuery.GetMyQuestionsToBeAnsweredWithInvitedMe(UserId, pageIndex, pageSize);
            return ResponseResult.Success(r);
        }


        /// <summary>
        /// 我的提问
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("questions")]
        [ProducesResponseType(typeof(APIResult<Page<QaQuestionListItemDto>>), 200)]
        public async Task<ResponseResult> GetMyQuestions(int pageIndex, int pageSize)
        {
            var r = await _questionQuery.GetMyQuestions(UserId, pageIndex, pageSize);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 我的回答
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("answers")]
        [ProducesResponseType(typeof(APIResult<Page<QaQuestionListItemDto>>), 200)]
        public async Task<ResponseResult> GetMyAnswers(int pageIndex, int pageSize)
        {
            var r = await _answerQuery.GetMyAnswers(UserId, pageIndex, pageSize);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 我的收藏-问答帖子
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("collect/questions")]
        [ProducesResponseType(typeof(APIResult<Page<MyCollectQuestionListItemDto>>), 200)]
        public async Task<ResponseResult> GetMyCollectQuestions(int pageIndex, int pageSize)
        {
            var r = await _questionQuery.GetMyCollectQuestions(UserId, pageIndex, pageSize);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 我的收藏-专栏
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("collect/subjects")]
        [ProducesResponseType(typeof(APIResult<Page<SubjectItemDto>>), 200)]
        public async Task<ResponseResult> GetMyCollectSubjects(int pageIndex, int pageSize)
        {
            var r = await _subjectQuery.GetMyCollectSubjects(UserId, pageIndex, pageSize);
            return ResponseResult.Success(r);
        }
    }
}
