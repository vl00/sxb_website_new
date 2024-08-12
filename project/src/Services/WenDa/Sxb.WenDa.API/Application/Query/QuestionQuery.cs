using Sxb.WenDa.Common.Enum;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Extensions;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Common.ResponseDto.Home;
using Sxb.WenDa.Common.OtherAPIClient.School;
using Sxb.WenDa.Common.OtherAPIClient.User;
using Sxb.WenDa.Query.SQL.Repositories;
using Sxb.WenDa.Common.OtherAPIClient.User.Models;

namespace Sxb.WenDa.API.Application.Query
{
    public partial class QuestionQuery : IQuestionQuery
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ICityCategoryQuery _cityCategoryQuery;
        private readonly ICityCategoryRepository _cityCategoryRepository;
        private readonly ISchoolApiService _schoolApiService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IUserApiService _userApiService;
        private readonly IServiceProvider _services;
        private readonly IAnswerRepository _answerRepository;
        private readonly IUserCategoryAttentionRepository _userCategoryAttentionRepository;
        private readonly ISchoolQuestionRepository _schoolQuestionRepository;
        private readonly ICustomLanmuDataRepository _customLanmuDataRepository;
        private readonly ICategoryRepository _categoryRepository;

        public QuestionQuery(IQuestionRepository questionRepository, ISchoolApiService schoolApiService, IAnswerRepository answerRepository,
            ICityCategoryQuery cityCategoryQuery, ICityCategoryRepository cityCategoryRepository, IUserApiService userApiService,
            IEasyRedisClient easyRedisClient, IUserCategoryAttentionRepository userCategoryAttentionRepository,
            ISchoolQuestionRepository schoolQuestionRepository, ICustomLanmuDataRepository customLanmuDataRepository,
            ICategoryRepository categoryRepository,
            IServiceProvider services)
        {
            _questionRepository = questionRepository;
            _cityCategoryQuery = cityCategoryQuery;
            _cityCategoryRepository = cityCategoryRepository;
            _schoolApiService = schoolApiService;
            this._easyRedisClient = easyRedisClient;
            _userApiService = userApiService;
            _answerRepository = answerRepository;
            _services = services;
            _userCategoryAttentionRepository = userCategoryAttentionRepository;
            _schoolQuestionRepository = schoolQuestionRepository;
            _customLanmuDataRepository = customLanmuDataRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Guid> GetQuestionIdByNo(long no)
        {
            if (no == default) throw new ResponseResultException("", 201);
            if (no != default)
            {
                var id1 = await _easyRedisClient.GetStringAsync(string.Format(CacheKeys.QuestionNo2Id, no));
                if (!string.IsNullOrEmpty(id1))
                {
                    return Guid.Parse(id1);
                }
            }
            if (no != default)
            {
                var dto = await _questionRepository.GetQuestion(no: no);
                if (dto == null) throw new ResponseResultException("问题不存在", Errcodes.Wenda_QuestionNotExists);

                var tasks = new List<Task>();
                var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                tasks.Add(batch.StringSetAsync(string.Format(CacheKeys.QuestionNo2Id, no), $"{dto.Id}", TimeSpan.FromDays(1)));
                batch.Execute();
                await Task.WhenAll(tasks);

                return dto.Id;
            }
            throw new ResponseResultException("", 201);
        }

        public async Task<IEnumerable<QuestionDbDto>> GetQuestionDbDtos(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default)
        {
            var lsRes = await _easyRedisClient.GetItemsByIdsOrNos(ids, nos,
                no => string.Format(CacheKeys.QuestionNo2Id, no),
                id => string.Format(CacheKeys.Question, id),
                (ids2, nos2) => _questionRepository.LoadQuestions(ids2, nos2),
                item => (item.Id, item.No),
                60 * 60 * 1
            );

            return lsRes.DistinctBy(_ => _.Id);
        }

        public async Task<IEnumerable<QaQuestionListItemDto>> GetQaQuestionListItemDtos(IEnumerable<Guid> ids, Guid userId)
        {
            var data = await GetQaItemDtos<QaQuestionListItemDto>(ids, userId: userId);
            data = data.ItemsOrderBy(ids, _ => _.QuestionId);

            await FillQaQuestionListItemDtoByMaxLikeCountAnswer(data, userId);
            return data;
        }

        public async Task<IEnumerable<T>> GetQaItemDtos<T>(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default, Guid userId = default)
            where T : QaItemDto, new()
        {
            var lsRes = await GetQuestionDbDtos(ids, nos);
            // found
            var result = lsRes.Select(_ => Convert_to_QaItemDto<T>(_, userId)).ToList();

            // find school
            var eids = result.SelectMany(_ => _._SchoolEids ?? new List<Guid>()).Distinct();
            if (eids.Any())
            {
                var schools = await _schoolApiService.GetSchoolsIdAndName(eids.Select(_ => _.ToString()));
                if (schools?.Count > 0)
                {
                    foreach (var dto in result)
                    {
                        for (int i = 0, len = (dto._SchoolEids?.Count ?? 0); i < len; i++)
                        {
                            var eid = dto._SchoolEids[i];
                            if (!schools.TryGetOne(out var x, _ => _.Eid == eid)) continue;
                            dto.SchoolIds ??= new List<string>(dto._SchoolEids.Count);
                            dto.SchoolNames ??= new List<string>(dto._SchoolEids.Count);
                            dto.SchoolIds.Add(x.SchoolNo);
                            dto.SchoolNames.Add($"{x.Schname}-{x.Extname}");
                        }
                        //dto.SchoolIds?.RemoveAll(_ => _ == null);
                        //dto.SchoolNames?.RemoveAll(_ => _ == null);
                    }
                }
            }

            // users
            if (result.Count > 0)
            {
                var rr = await _userApiService.GetUsersDesc(result.Where(_ => _.IsAnony != true).Select(_ => _.UserId).Distinct());
                foreach (var dto in result)
                {
                    if (dto.IsAnony == true) continue;
                    if (!rr.TryGetOne(out var x, _ => _.Id == dto.UserId)) continue;
                    dto.UserName = string.IsNullOrEmpty(x.Name) ? dto.UserName : x.Name;
                    dto.UserHeadImg = string.IsNullOrEmpty(x.HeadImg) ? dto.UserHeadImg : x.HeadImg;
                    dto.UserDesc = x.CertificationPreview;
                }
            }

            // CollectCount is got in cache before
            //
            // IsCollectedByMe
            for (var ___ = result.Count > 0; ___; ___ = false) // if break
            {
                if (typeof(T) == typeof(MyCollectQuestionListItemDto))
                {
                    foreach (var dto in result.OfType<MyCollectQuestionListItemDto>())
                    {
                        dto.IsCollectedByMe = dto.CollectCount > 0 ? true : default;
                    }
                    break;
                }

                if (userId == default) break;

                var rr = await _questionRepository.IsCollectedByMe(userId, result.Select(_ => _.QuestionId).Distinct());
                foreach (var dto in result)
                {
                    if (!rr.TryGetOne(out var x, _ => _.Qid == dto.QuestionId)) continue;
                    dto.IsCollectedByMe = x.IsCollectedByMe;
                }
            }

            // answer
            // answer部分后续根据需要填充。。

            foreach (var dto in result)
            {
                dto._SchoolEids = null;
                dto.Tags = dto.Tags?.Any() == true ? dto.Tags : null;
            }
            return result;
        }

        static T Convert_to_QaItemDto<T>(QuestionDbDto s, Guid userId)
            where T : QaItemDto, new()
        {
            var t = new T();
            t.QuestionId = s.Id;
            t.QuestionNo = UrlShortIdUtil.Long2Base32(s.No);
            t.Title = s.Title;
            t.City = s.City;
            t.ReplyCount = s.ReplyCount;
            t.CollectCount = s.CollectCount;
            t.CreateTime = s.CreateTime;
            t.EditTime = s.LastEditTime;
            t.UserId = s.UserId;
            t.IsMyQuestion = s.UserId == userId;
            t.Tags = s.TagNames?.Where(_ => !string.IsNullOrEmpty(_))?.ToList();
            t.CityName = s.CityName;
            t._SchoolEids = s.Eids?.ToList();
            t.IsAnony = s.IsAnony;
            if (t.IsAnony == true) t.UserName = s.AnonyUserName ?? $"匿名用户{t.QuestionId.ToString("n")[..7]}";
            else t.UserName = $"用户{t.QuestionId.ToString("n")[..7]}";
            t.UserHeadImg = BusinessLogicUtils.AnonyUserHeadImg;
            switch (t)
            {
                case QaQuestionItemDto t2:
                    t2.Content = s.Content;
                    t2.Imgs = s.Imgs?.JsonStrTo<string[]>();
                    t2.Imgs_s = s.Imgs_s?.JsonStrTo<string[]>();
                    t2.CategoryId = s.CategoryId ?? 0;
                    t2.CategoryName = s.CategoryName;
                    t2.TagIds = s.TagIds?.AsArray();
                    t2.Platform = s.Platform;
                    break;
            }
            return t;
        }

        public async Task FillQaQuestionListItemDtoByQaIds(IEnumerable<QaQuestionListItemDto> qItems, IEnumerable<(Guid Qid, Guid Aid)> qaIds, Guid userId = default)
        {
            var _answerQuery = _services.GetService<IAnswerQuery>();

            if (qItems.Any())
            {
                var ls_a = await _answerQuery.GetQaAnswerItemDtos(ids: qaIds.Select(_ => _.Aid), userId: userId);
                foreach (var dto in qItems)
                {
                    if (!qaIds.TryGetOne(out var x, _ => _.Qid == dto.QuestionId)) continue;
                    if (!ls_a.TryGetOne(out var a, _ => _.AnswerId == x.Aid)) continue;
                    dto.Answer = a;
                }
            }
        }

        public async Task FillQaQuestionListItemDtoByMaxLikeCountAnswer(IEnumerable<QaQuestionListItemDto> qItems, Guid userId = default)
        {
            var _answerQuery = _services.GetService<IAnswerQuery>();

            for (var ___ = qItems.Any(); ___; ___ = !___) // if break
            {
                var qaIds = await _answerRepository.GetAnswersIdsWithMaxLikeCountInQuestion(qItems.Select(_ => _.QuestionId));
                if (qaIds?.Any() != true) break;
                await FillQaQuestionListItemDtoByQaIds(qItems, qaIds, userId);
            }
        }


        public async Task<GetQuestionByKeywordQueryResult> GetQuestionByKeyword(GetQuestionByKeywordQuery query)
        {
            var result = new GetQuestionByKeywordQueryResult();
            result.Items = await _questionRepository.GetTopNQuestionByKeywords(query.Keyword);
            return result;
        }

        public async Task<Page<QaQuestionListItemDto>> GetMyQuestions(Guid userId, int pageIndex, int pageSize)
        {
            var _answerQuery = _services.GetService<IAnswerQuery>();

            var pg_qids = await _questionRepository.GetMyQuestionsIds(userId, pageIndex, pageSize);
            var qids = pg_qids.Data.Distinct();

            var ls_q = await GetQaItemDtos<QaQuestionListItemDto>(qids, userId: userId);
            ls_q = ls_q.ItemsOrderBy(qids, _ => _.QuestionId).AsArray();

            await FillQaQuestionListItemDtoByMaxLikeCountAnswer(ls_q, userId);

            return new(ls_q, pg_qids.Total);
        }

        public async Task<Page<MyCollectQuestionListItemDto>> GetMyCollectQuestions(Guid me, int pageIndex, int pageSize)
        {
            var _answerQuery = _services.GetService<IAnswerQuery>();

            var pg_qids = await _questionRepository.GetUserCollectQuestions(me, pageIndex, pageSize);
            var qids = pg_qids.Data.Distinct();

            var ls_q = await GetQaItemDtos<MyCollectQuestionListItemDto>(qids, userId: me);
            ls_q = ls_q.ItemsOrderBy(qids, _ => _.QuestionId).AsArray();

            await FillQaQuestionListItemDtoByMaxLikeCountAnswer(ls_q, me);

            // 是否我回答过
            if (ls_q.Any())
            {
                var rr = await _answerRepository.GetQuestionsIfHasMyAnswers(me, qids);
                foreach (var dto in ls_q)
                {
                    if (!rr.TryGetOne(out var x, _ => _.Qid == dto.QuestionId)) dto.HasMyAnswers = false;
                    dto.HasMyAnswers = x.HasMyAnswers;
                }
            }

            return new(ls_q, pg_qids.Total);
        }

        public async Task<Page<ToBeAnsweredQuestionListItemDto>> GetMyQuestionsToBeAnsweredWithInvitedMe(Guid me, int pageIndex, int pageSize)
        {
            var pg_qids = await _questionRepository.GetUserQuestionsToBeAnsweredByInvited(me, pageIndex, pageSize);
            var qids = pg_qids.Data.Distinct();

            var ls_q = await GetQaItemDtos<ToBeAnsweredQuestionListItemDto>(qids);
            ls_q = ls_q.ItemsOrderBy(qids, _ => _.QuestionId).AsArray();

            return new(ls_q, pg_qids.Total);
        }

        public async Task<QuestionsToBeAnsweredVm> GetMyQuestionsToBeAnswered(Guid me, int pageIndex, int pageSize)
        {
            var _userCategoryAttentionQuery = _services.GetService<IUserCategoryAttentionQuery>();

            var depth2CategoryIds = await _userCategoryAttentionRepository.GetUserCategoryIdsAsync(me);
            var myQuesCategorys = await _categoryRepository.GetQuesCategoryIdsByAttentionCategoryIds(depth2CategoryIds);
            var platforms = myQuesCategorys.Select(_ => BusinessLogicUtils.GetPlatformFromCategoryPath(_.Path)).Distinct();

            var pgQids = await _questionRepository.GetUserQuestionsToBeAnswered(me, myQuesCategorys.Select(_ => _.Id), platforms, pageIndex, pageSize);
            var qids = pgQids.Data.Distinct();

            var lsQ = await GetQaItemDtos<ToBeAnsweredQuestionListItemDto>(qids);
            lsQ = lsQ.ItemsOrderBy(qids, _ => _.QuestionId).AsArray();

            var result = new QuestionsToBeAnsweredVm();
            result.Data = lsQ;
            result.Total = pgQids.Total;

            if (pageIndex == 1)
            {
                result.UserCategoryAttentions = await _userCategoryAttentionQuery.GetUserCategories(me);
                result.UserCategoryAttentions = result.UserCategoryAttentions?.AsList();

                result.InvitedMePage1 = await GetMyQuestionsToBeAnsweredWithInvitedMe(me, pageIndex, pageSize);
            }

            return result;
        }

        public async Task<IEnumerable<RelevantQuestionDto>> GetTopNRelevantQuestions(int top = 6, long? city = null, IEnumerable<long> tagIds = null, IEnumerable<long> categoryIds = null, Guid? qidNotIn = null)
        {
            var keyCache = tagIds?.Any() == true ? string.Format(CacheKeys.RelevantQuestions, city, string.Join('|', tagIds.OrderBy(_ => _), null), qidNotIn)
                : string.Format(CacheKeys.RelevantQuestions, city, null, categoryIds?.Any() != true ? "" : string.Join('|', categoryIds.OrderBy(_ => _)), qidNotIn);
            var ls = await _easyRedisClient.GetAsync<List<RelevantQuestionDto>>(keyCache);
            if (ls == null)
            {
                ls = (await _questionRepository.GetTopNRelevantQuestions(6, city, tagIds, categoryIds, (qidNotIn == null ? null : new[] { qidNotIn.Value }))).AsList();

                await _easyRedisClient.AddAsync(keyCache, ls, TimeSpan.FromSeconds(60 * 60 * 2));
            }
            return ls;
        }

        public async Task<Page<QaQuestionListItemDto>> GetEveryoneTalkingAboutPageList(int pageIndex, int pageSize, Guid userId = default)
        {
            var keyCache = string.Format(CacheKeys.EventOneTalkAboutLs, pageIndex, pageSize);
            Page<Guid> pgIds = default!;
            if (pageIndex == 1)
            {
                pgIds = await _easyRedisClient.GetAsync<Page<Guid>>(keyCache);
            }
            if (pgIds == default)
            {
                pgIds = await _questionRepository.GetEveryoneTalkingAboutPageList(pageIndex, pageSize);

                if (pageIndex == 1)
                {
                    await _easyRedisClient.AddAsync(keyCache, pgIds, TimeSpan.FromSeconds(60 * 60 * 12));
                }
            }

            var items = await GetQaItemDtos<QaQuestionListItemDto>(pgIds.Data, userId: userId);
            items = items.ItemsOrderBy(pgIds.Data, _ => _.QuestionId).AsList();

            await FillQaQuestionListItemDtoByMaxLikeCountAnswer(items, userId);

            return new(items, pgIds.Total);
        }


        /// <summary>
        /// 获取等你来回答
        /// 
        /// 若用户已设置擅长领域。
        ///  1、优先显示学段分类重合度高的问题。
        ///  2、在1的前提下，优先显示真实用户发起的提问。
        ///  3、在1和2的前提下，优先显示回答数少的问题。
        ///  若用户未设置擅长领域。
        ///  1、优先显示真实用户发起的提问。
        ///  2、在1的前提下优先显示回答数少的问题。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<List<ToBeAnsweredQuestionListItemDto>> GetWaitQuestions(Guid? userId, int top = 5)
        {
            top = top > 20 ? 5 : top;
            var key = string.Format(CacheKeys.HomeWaitQuestions, userId);
            await _easyRedisClient.RemoveAsync(key);
            return await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                //var hotQuestions = (await _questionRepository.GetWaitsAsync(categoryIds, exclundUserId: userId.GetValueOrDefault(), top)).ToList();
                //invite ids
                var ids = (await _questionRepository.GetUserQuestionsToBeAnsweredByInvited(userId: userId.GetValueOrDefault(), pageIndex:1, top))
                    .Data.ToList();
                var surplus = top - ids.Count;
                if (surplus > 0)
                {
                    IEnumerable<long> categoryIds = userId == null ? Enumerable.Empty<long>()
                             : await _userCategoryAttentionRepository.GetUserCategoryIdsAsync(userId.Value);

                    var myQuesCategorys = await _categoryRepository.GetQuesCategoryIdsByAttentionCategoryIds(categoryIds);
                    var platforms = myQuesCategorys.Select(_ => BusinessLogicUtils.GetPlatformFromCategoryPath(_.Path)).Distinct();

                    var waitIds = (await _questionRepository.GetUserQuestionsToBeAnswered(userId: userId.GetValueOrDefault(), myQuesCategorys.Select(_ => _.Id), platforms, pageIndex: 1, surplus))
                        .Data;
                    ids.AddRange(waitIds);
                }
                var data = await GetQaItemDtos<ToBeAnsweredQuestionListItemDto>(ids);
                return data.ItemsOrderBy(ids, _ => _.QuestionId).ToList();
            }, TimeSpan.FromMinutes(60));
        }

        /// <summary>
        /// 获取热门学校和他的最新问题
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="top"></param>
        /// <param name="questionTop"></param>
        /// <returns></returns>
        public async Task<IEnumerable<HotQuestionSchoolItemDto>> GetSchoolsAsync(ArticlePlatform platform, int top, int questionTop = 6)
        {
            questionTop = questionTop > 20 ? 6 : questionTop;

            var extIds = await _schoolQuestionRepository.GetHotSchoolIdsAsync(platform, top);
            var schoolds = await _schoolApiService.GetSchoolsIdAndName(extIds);

            var questions = await _questionRepository.GetSchoolQuestionLinksAsync(extIds, QuestionOrderBy.CreateTimeDesc, questionTop);

            return schoolds.Select(s => new HotQuestionSchoolItemDto()
            {
                SchoolNo = s.SchoolNo,
                SchoolName = s.SchoolName,
                Questions = questions.Where(q => q.ExtId == s.Eid)
                                .Select(q => new QuestionLinkDto(q))
                                .ToList()
            });
        }


        /// <summary>
        /// 获取随机推荐列表
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<IEnumerable<QuestionCategoryItemDto>> GetRandomRecommendAsync(int top)
        {
            top = top > 1000 ? 210 : top;
            var key = string.Format(CacheKeys.HomeRandomRecommendQuestionss, top);
            var data = await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                //取双倍
                return (await _questionRepository.GetRandomRecommendAsync(top * 2)).ToList();
            }, TimeSpan.FromMinutes(60));

            CommonHelper.ListRandom(data);

            //取top
            return data.Take(top);
        }


        /// <summary>
        /// 获取子站热门推荐
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="city"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<IEnumerable<QuestionLinkDto>> GetRecommendsAsync(ArticlePlatform platform, int city, int top)
        {
            var key = string.Format(CacheKeys.HomeHotRecommendQuestions, platform, city);
            var ids = await _customLanmuDataRepository.GetLanmuData<List<Guid>>(key);

            if (!ids.Any())
            {
                var page = await _questionRepository.GetQuestionPageAsync(
                                    platform,
                                    city,
                                    categoryIds: null,
                                    pageIndex: 1,
                                    pageSize: top);

                return page.Data.Select(s => new QuestionLinkDto(s));
            }
            else
            {
                var questions = await _questionRepository.GetQuestionsAsync(ids);
                return questions.Select(s => new QuestionLinkDto(s));
            }
        }


        /// <summary>
        /// 获取问答列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Page<QaQuestionListItemDto>> GetQuestionPageAsync(QaListRequestDto request, Guid userId)
        {
            List<Guid> ids = new List<Guid>();
            var categoryIds = request.CategoryId > 0
                ? new List<long>() { request.CategoryId } : null;

            //查询热门推荐的
            var key = request.CategoryId == 0
                ? string.Format(CacheKeys.HomeHotRecommendQuestions, request.Platform, request.City)
                : string.Format(CacheKeys.Questions, request.Platform, request.City, request.CategoryId);

            var configTotal = 0;
            //第一页, 如果有配置的优先展示
            if (request.PageIndex == 1)
            {
                var configIds = await _customLanmuDataRepository.GetLanmuData<List<Guid>>(key);
                ids.AddRange(configIds);
                configTotal = configIds.Count;
            }

            var page = await _questionRepository.GetQuestionPageAsync(
                                request.Platform,
                                request.City,
                                categoryIds,
                                request.PageIndex,
                                request.PageSize);

            ids.AddRange(page.Data.Select(s => s.Id));

            var data = await GetQaQuestionListItemDtos(ids, userId);

            //return data.ToPage(page.Total + configTotal);
            return page.ChangeData(data);
        }
    }
}
