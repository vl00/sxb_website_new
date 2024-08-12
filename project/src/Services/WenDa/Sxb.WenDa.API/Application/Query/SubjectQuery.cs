using MediatR;
using StackExchange.Redis;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Extensions;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Common.OtherAPIClient.School;
using Sxb.WenDa.Common.OtherAPIClient.User;
using Sxb.WenDa.Query.SQL.Repositories;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.API.Application.Commands;

namespace Sxb.WenDa.API.Application.Query
{
    public partial class SubjectQuery : ISubjectQuery
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ICityCategoryQuery _cityCategoryQuery;
        private readonly ICityCategoryRepository _cityCategoryRepository;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IUserApiService _userApiService;
        private readonly IQaSubjectRepository _subjectRepository;
        private readonly IServiceProvider _services;
        private readonly IMediator _mediator;
        private readonly IUserCollectInfoRepository _userCollectInfoRepository;

        public SubjectQuery(IQuestionRepository questionRepository, IQaSubjectRepository subjectRepository, IMediator mediator,
            ICityCategoryQuery cityCategoryQuery, ICityCategoryRepository cityCategoryRepository, IUserApiService userApiService,
            IEasyRedisClient easyRedisClient, IServiceProvider services, IUserCollectInfoRepository userCollectInfoRepository)
        {
            _questionRepository = questionRepository;
            _cityCategoryQuery = cityCategoryQuery;
            _cityCategoryRepository = cityCategoryRepository;
            _subjectRepository = subjectRepository;
            this._easyRedisClient = easyRedisClient;
            _userApiService = userApiService;
            _services = services;
            _mediator = mediator;
            _userCollectInfoRepository = userCollectInfoRepository;
        }

        public async Task<SubjectItemDto> GetSubjectItem(Guid id = default, long no = default)
        {
            if (id != default && no != default) throw new ResponseResultException("", 201);
            if (id == default && no == default) throw new ResponseResultException("", 201);

            return (await GetSubjectItems(new[] { id }, new[] { no })).FirstOrDefault();
        }


        public async Task<IEnumerable<SubjectItemDto>> GetSubjectItems(IEnumerable<Guid> ids, Guid userId)
        {
            var subjects = await GetSubjectItems(ids);
            subjects = subjects.ItemsOrderBy(ids, _ => _.SubjectId).ToList();
            var collectedIds = await _userCollectInfoRepository.GetCollectedDataIds(UserCollectType.Subject, ids, userId);

            return subjects.Select(subject =>
            {
                subject.IsCollectedByMe = collectedIds.Any(id => id == subject.SubjectId);
                return subject;
            }).ToList();
        }


        public async Task<IEnumerable<SubjectItemDto>> GetSubjectItems(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default)
        {
            var lsRes = await _easyRedisClient.GetItemsByIdsOrNos(ids, nos,
                no => string.Format(CacheKeys.SubjectNo2Id, no),
                id => string.Format(CacheKeys.Subject, id),
                (ids2, nos2) => _subjectRepository.GetSubjects(ids2, nos2),
                item => (item.Id, item.No),
                60 * 60 * 1
            );

            // found
            var result = lsRes.Select(_ => Convert_to_SubjectItemDto(_)).ToList();
            return result;
        }

        static SubjectItemDto Convert_to_SubjectItemDto(SubjectDbDto dto)
        {
            var result = new SubjectItemDto();
            result.SubjectId = dto.Id;
            result.SubjectNo = UrlShortIdUtil.Long2Base32(dto.No);
            result.Title = dto.Title;
            result.Content = dto.Content;
            result.Img = dto.Img;
            result.Img_s = dto.Img_s;
            result.CategoryIds = dto.CategoryIds;
            result.CategoryNames = dto.CategoryNames;
            result.TagIds = dto.TagIds;
            result.TagNames = dto.TagNames;
            result.ViewCount = dto.ViewCount; //
            result.CollectCount = dto.CollectCount; //
            result.IsCollectedByMe = default; // 需要后续查
            return result;
        }

        public async Task<SubjectDetailVm> GetSubjectDetail(string id, long? city = null, Guid? me = null)
        {
            var result = new SubjectDetailVm();

            var subjectId = Guid.TryParse(id, out var _id) ? _id : default;
            var subjectNo = subjectId == default ? UrlShortIdUtil.Base322Long(id) : default;
            result.Subject = await GetSubjectItem(subjectId, subjectNo);
            subjectId = result.SubjectId;

            if (me != null)
            {
                result.Subject.IsCollectedByMe = await _subjectRepository.IsCollectedByUser(subjectId, me.Value);
            }

            // city
            {
                var citys = await _cityCategoryQuery.GetCitys();
                result.Cities = citys.AsArray();
            }

            // viewcount
            {
                var rr = (await GetSubjectsViewCounts(new[] { subjectId })).FirstOrDefault();
                result.Subject.ViewCount = rr.Item2 + 1;

                await _mediator.Send(new UpSubjectViewCountCommand { SubjectId = subjectId, IncrCount = 1, UserId = me ?? default });
            }

            // RelevantQuestions
            // 前端另外调用接口
            {
                //var key_cache = string.Format(CacheKeys.Subject_RelevantQuestions, result.SubjectId);
                //result.RelevantQuestions = await _easyRedisClient.GetAsync<RelevantQuestionDto[]>(key_cache);
                //if (result.RelevantQuestions == null)
                //{
                //    var ls = await _questionRepository.GetTopNRelevantQuestions(6, city, result.Subject.CategoryIds);

                //    await _easyRedisClient.AddAsync(key_cache, ls, TimeSpan.FromSeconds(60 * 60 * 2));

                //    result.RelevantQuestions = ls.AsArray();
                //}
            }

            // RecommendArticles
            // 需要问答分类跟文章分类对应关系

            return result;
        }

        public async Task<Page<SubjectItemDto>> GetMyCollectSubjects(Guid me, int pageIndex, int pageSize)
        {
            var page = await _subjectRepository.GetUserCollectSubjectIds(me, pageIndex, pageSize);

            var items = await GetSubjectItems(page.Data);
            var items2 = items.ItemsOrderBy(page.Data, _ => _.SubjectId).AsArray();

            foreach (var dto in items2)
            {
                dto.IsCollectedByMe = true;
            }

            // viewcount
            {
                var rr = await GetSubjectsViewCounts(page.Data);
                foreach (var item in items2)
                {
                    if (!rr.TryGetOne(out var x, _ => _.Item1 == item.SubjectId)) continue;
                    item.ViewCount = x.Item2;
                }
            }

            return new(items2, page.Total);
        }

        public async Task<Page<QaQuestionListItemDto>> GetQuestionsBySubject(GetQuestionsPageListBySubjectQuery query)
        {
            var _questionQuery = _services.GetService<IQuestionQuery>();
            query.Orderby = query.Orderby > 0 ? query.Orderby : 1;

            var subjectId = Guid.TryParse(query.SubjectId, out var _id) ? _id : default;
            var subjectNo = subjectId == default ? UrlShortIdUtil.Base322Long(query.SubjectId) : default;
            var subject = await GetSubjectItem(subjectId, subjectNo);

            var keyCache = string.Format(CacheKeys.SubjectQuestionsPageList, subject.SubjectId, query.PageIndex, query.PageSize, query.Orderby);
            Page<Guid> pgIds = default!;
            if (query.PageIndex == 1)
            {
                pgIds = await _easyRedisClient.GetAsync<Page<Guid>>(keyCache);
            }
            if (pgIds == default)
            {
                pgIds = await _subjectRepository.GetQuestionsIdsPageListBySubject(subject.SubjectId, query.PageIndex, query.PageSize, (SubjectQuestionListOrderByEnum)query.Orderby);

                if (query.PageIndex == 1)
                {
                    await _easyRedisClient.AddAsync(keyCache, pgIds, TimeSpan.FromSeconds(60 * 60 * 3));
                }
            }

            var lsQ = await _questionQuery.GetQaItemDtos<QaQuestionListItemDto>(pgIds.Data, userId: query.UserId);
            lsQ = lsQ.ItemsOrderBy(pgIds.Data, _ => _.QuestionId).AsArray();
            await _questionQuery.FillQaQuestionListItemDtoByMaxLikeCountAnswer(lsQ, query.UserId);

            return new(lsQ, pgIds.Total);
        }

        public async Task<IEnumerable<(Guid, int)>> GetSubjectsViewCounts(IEnumerable<Guid> ids)
        {
            ids = (ids ?? Enumerable.Empty<Guid>()).Distinct();
            List<(Guid, int)> result = new();
            List<(Guid, int)> rr2 = new();

            // find in cache
            if (ids.Any())
            {
                var tasks = new List<Task<(Guid, int)>>();
                var tasksIncr = new List<Task<(Guid, int)>>();
                var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                foreach (var id in ids)
                {
                    var t = batch.StringGetAsync(string.Format(CacheKeys.SubjectViewCount0, id)).ContinueWith(t => (id, int.TryParse(t.Result, out var _i) ? _i : -1));
                    tasks.Add(t);
                    t = batch.StringGetAsync(string.Format(CacheKeys.SubjectViewCountIncr, id)).ContinueWith(t => (id, int.TryParse(t.Result, out var _i) ? _i : 0));
                    tasksIncr.Add(t);
                }
                batch.Execute();
                await Task.WhenAll(tasks.Union(tasksIncr));

                result.AddRange(tasks.Select(_ => _.Result).Where(_ => _.Item2 != -1).ToList());
                rr2.AddRange(tasksIncr.Select(_ => _.Result).ToList());

                ids = tasks.Select(_ => _.Result).Where(_ => _.Item2 == -1).Select(_ => _.Item1);
            }
            // find in db
            if (ids.Any())
            {
                var ls = await _subjectRepository.GetSubjectsViewCounts(ids);
                result.AddRange(ls);

                var tasks = new List<Task>();
                var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                foreach (var dto in ls)
                {
                    var t = batch.StringSetAsync(string.Format(CacheKeys.SubjectViewCount0, dto.Item1), $"{dto.Item2}", TimeSpan.FromSeconds(60 * 60 * 2), When.NotExists);
                    tasks.Add(t);
                }
                batch.Execute();
                await Task.WhenAll(tasks);
            }

            return result.Select(x => (x.Item1, x.Item2 + rr2.FirstOrDefault(_ => _.Item1 == x.Item1).Item2));
        }
    }
}
