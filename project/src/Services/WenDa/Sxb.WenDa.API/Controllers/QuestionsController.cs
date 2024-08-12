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
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Enum;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.API.Controllers
{
    /// <summary>
    /// 问题
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ApiControllerBase
    {
        private readonly IQuestionQuery _questionQuery;
        private readonly IMediator _mediator;
        private readonly IEasyRedisClient _easyRedisClient;

        public QuestionsController(IQuestionQuery questionQuery, IMediator mediator,
            IEasyRedisClient easyRedisClient)
        {
            _questionQuery = questionQuery;
            _mediator = mediator;
            _easyRedisClient = easyRedisClient;
        }


        /// <summary>
        /// 大家热议 (不含列表)
        /// </summary>
        /// <returns></returns>
        [HttpGet("EveryoneTalkingAbout/detail")]
        [ProducesResponseType(typeof(APIResult<EveryoneTalkingAboutDetailVm>), 200)]
        public async Task<ResponseResult> EveryoneTalkingAboutDetail()
        {
            var r = await _questionQuery.GetEveryoneTalkingAboutDetail();
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 大家热议-列表分页
        /// </summary>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        [HttpGet("EveryoneTalkingAbout/pagelist")]
        [ProducesResponseType(typeof(APIResult<Page<QaQuestionListItemDto>>), 200)]
        public async Task<ResponseResult> GetEveryoneTalkingAboutPageList(int pageIndex = 1, int pageSize = 20)
        {
            var r = await _questionQuery.GetEveryoneTalkingAboutPageList(pageIndex, pageSize, UserId);
            return ResponseResult.Success(r);
        }



        /// <summary>
        /// 加载问题用于编辑
        /// </summary>
        /// <param name="id">
        /// 问题id <br/>
        /// 编辑问题时,必传 <br/>
        /// 发问题时,不传.可获取城市数据
        /// </param>
        /// <returns></returns>
        [HttpGet("load")]
        [ProducesResponseType(typeof(APIResult<LoadQuestionForSaveDto>), 200)]
        public async Task<ResponseResult> LoadQuestionForSave(Guid? id = null)
        {
            var r = await _questionQuery.GetQuestionByIdForSave(id);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 发问题-搜索问题
        /// </summary>
        /// <param name="kw">搜索关键词</param>
        /// <returns></returns>
        [HttpGet("getbykeyword")]
        [ProducesResponseType(typeof(APIResult<GetQuestionByKeywordQueryResult>), 200)]
        public async Task<ResponseResult> GetQuestionByKeyword(string kw)
        {
            var r = await _questionQuery.GetQuestionByKeyword(new GetQuestionByKeywordQuery { Keyword = Uri.UnescapeDataString(kw) });
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 发问题-查城市下的分类+标签
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>        
        [HttpGet("citycategory")]
        [ProducesResponseType(typeof(APIResult<GetCityCategoryQueryResult>), 200)]        
        public async Task<ResponseResult> GetCityCategory([FromQuery] GetCityCategoryQuery query, 
            [FromServices] ICityCategoryQuery cityCategoryQuery)
        {
            var r = await cityCategoryQuery.GetCityCategory(query);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 发问题
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Authorize]        
        [HttpPost("add")]
        [CheckBindMobile, CheckBindWeixin]
        [ResponseResultExceptionToResult]
        [ProducesResponseType(typeof(APIResult<AddQuestionCommandResult>), 200)]
        public async Task<ResponseResult> AddAsync(AddQuestionCommand cmd)
        {
            cmd.UserId = UserId;

            var lck1f = new StecRedisLock1Factory1(_easyRedisClient);
            var k = string.Format(CacheKeys.Wenda_lck_addques_title, MD5Helper.GetMD5(cmd.Title));
            await using var lck1 = await lck1f.LockAsync(new Lock1Option(k).SetExpExpectBySec());
            if (!lck1.IsAvailable) throw new ResponseResultException("系统繁忙", Errcodes.Wenda_GetLck1Failed);

            var r =  await _mediator.Send(cmd);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 编辑问题
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("edit")]
        [CheckBindMobile, CheckBindWeixin]
        [ResponseResultExceptionToResult]
        [ProducesResponseType(typeof(APIResult<EditQuestionCommandResult>), 200)]
        public async Task<ResponseResult> EditAsync(EditQuestionCommand cmd)
        {
            cmd.UserId = UserId;

            var lck1f = new StecRedisLock1Factory1(_easyRedisClient);
            var k = string.Format(CacheKeys.Wenda_lck_question, cmd.Id);
            await using var lck1 = await lck1f.LockAsync(new Lock1Option(k).SetExpExpectBySec());
            if (!lck1.IsAvailable) throw new ResponseResultException("系统繁忙", Errcodes.Wenda_GetLck1Failed);

            var r = await _mediator.Send(cmd);
            return ResponseResult.Success(r);
        }



        /// <summary>
        /// 收藏/取消收藏
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("collect")]
        public async Task<ResponseResult> Collect([FromBody]CollectRequestDto request)
        {
            return await _mediator.Send(new CollectCommand()
            {
                Type = request.Type,
                UserId = UserId,
                DataId = request.DataId,
                IsValid = request.IsValid,
                CreateTime = DateTime.Now
            });
        }


        /// <summary>
        /// 获取问答列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(QaQuestionListItemDto), 200)]
        public async Task<ResponseResult> GetQuestionsAsync([FromQuery] QaListRequestDto request)
        {
            var data = await _questionQuery.GetQuestionPageAsync(request, UserId);
            return ResponseResult.Success(data);
        }


        /// <summary>
        /// 获取热门推荐
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="city"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        [HttpGet("recommends")]
        [ProducesResponseType(typeof(QuestionLinkDto), 200)]
        public async Task<ResponseResult> GetRecommendsAsync(ArticlePlatform platform, int city, int top)
        {
            var data = await _questionQuery.GetRecommendsAsync(platform,  city,  top);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 相关问题s
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost(nameof(GetRelevantQuestions))]
        [ProducesResponseType(typeof(RelevantQuestionsVm), 200)]
        public async Task<ResponseResult> GetRelevantQuestions(RelevantQuestionsQuery query)
        {
            var r = await _questionQuery.GetTopNRelevantQuestions(6, query.City, query.TagIds, query.CategoryIds, query.QidNotIn);
            return ResponseResult.Success(new RelevantQuestionsVm { RelevantQuestions = r.AsArray() });
        }
    }
}
