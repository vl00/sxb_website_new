using StackExchange.Redis;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Extensions;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.OtherAPIClient.User;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.Repositories;

namespace Sxb.WenDa.API.Application.Query
{
    public class AnswerQuery : IAnswerQuery
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IUserApiService _userApiService;
        private readonly IQuestionRepository _questionRepository;
        private readonly IServiceProvider _services;
        private readonly ICityCategoryRepository _cityCategoryRepository;
        private readonly IUserCategoryAttentionRepository _userCategoryAttentionRepository;
        private readonly IUserRepository _userRepository;

        public AnswerQuery(IAnswerRepository answerRepository, IUserApiService userApiService, IQuestionRepository questionRepository,
            IEasyRedisClient easyRedisClient, ICityCategoryRepository cityCategoryRepository, IUserCategoryAttentionRepository userCategoryAttentionRepository,
            IUserRepository userRepository,
            IServiceProvider services)
        { 
            _answerRepository = answerRepository;
            _easyRedisClient = easyRedisClient;
            _userApiService = userApiService;
            _questionRepository = questionRepository;
            _services = services;
            _cityCategoryRepository = cityCategoryRepository;
            _userCategoryAttentionRepository = userCategoryAttentionRepository;
            _userRepository = userRepository;
        }

        public async Task<LoadAnswerForSaveDto> GetAnswerByIdForSave(Guid id)
        {
            var dto = await _answerRepository.GetAnswer(id);
            if (dto?.IsValid != true) throw new ResponseResultException("回答不存在", Errcodes.Wenda_AnswerNotExists);
            //
            var result = new LoadAnswerForSaveDto();
            result.Id = dto.Id;
            result.Content = dto.Content;
            result.Imgs = dto.Imgs.FromJsonSafe<string[]>();
            result.Imgs_s = dto.Imgs_s.FromJsonSafe<string[]>();
            result.IsAnony = dto.IsAnony;
            return result;
        }

        public async Task<Page<QaAnswerItemDto>> GetAnswersPageList(Guid questionId, int pageIndex, int pageSize, AnswersListOrderByEnum orderby, Guid userId = default)
        {
            var re = 0;
            var keyCache = string.Format(CacheKeys.QuestionAnswersPageList, questionId, pageIndex, pageSize, (int)orderby);
            Page<Guid> pgIds = default!;
            if (pageIndex == 1)
            {
                pgIds = await _easyRedisClient.GetAsync<Page<Guid>>(keyCache);
            }
            LB_findInDB:
            if (pgIds == default)
            {
                re = 1;
                pgIds = await _answerRepository.GetAnswersPageList(questionId, pageIndex, pageSize, orderby);                

                if (pageIndex == 1)
                {
                    await _easyRedisClient.AddAsync(keyCache, pgIds, TimeSpan.FromSeconds(60 * 60 * 3));
                }
            }

            var items = await GetQaAnswerItemDtos(pgIds.Data, userId: userId);
            items = items.ItemsOrderBy(pgIds.Data, _ => _.AnswerId).AsList();

            for (var ___ = orderby == AnswersListOrderByEnum.LikeCountDesc; ___; ___ = false) // if break
            {
                var arr1 = pgIds.Data;
                var arr2 = items.OrderByDescending(_ => _.LikeCount).ThenByDescending(_ => _.CreateTime ?? default).Select(_ => _.AnswerId).ToArray();
                if (arr1.SequenceEqual(arr2)) break;
                if ((re++) < 1)
                {
                    pgIds = default;
                    goto LB_findInDB;
                }
                else
                {
                    items = items.OrderByDescending(_ => _.LikeCount).ThenByDescending(_ => _.CreateTime ?? default);
                }
            }

            return new(items, pgIds.Total);
        }

        public async Task<IEnumerable<QaAnswerItemDto>> GetQaAnswerItemDtos(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default, Guid userId = default)
        {
            List<(Guid Id, string)> lsCtn = new();

            var lsRes = await _easyRedisClient.GetItemsByIdsOrNos(ids, nos,
                (batch, no) => batch.StringGetAsync(string.Format(CacheKeys.AnswerNo2Id, no)).ContinueWith(t => (no, Guid.TryParse(t.Result.ToString(), out var _gid) ? _gid : default)),
                (batch, id) => batch.StringGetAsync(string.Format(CacheKeys.Answer, id)).ContinueWith(t => (id, t.Result.ToString().JsonStrTo<QuestionAnswer>())),
                (ids2, nos2) => _answerRepository.LoadQaAnswers(ids2, nos2),
                (tasks, batch, item) => 
                {
                    lsCtn.Add((item.Id, item.Content));
                    item.Content = null; // 富文本内容可能太大

                    tasks.Add(batch.StringSetAsync(string.Format(CacheKeys.AnswerNo2Id, item.No), item.Id.ToString(), TimeSpan.FromSeconds(60 * 60 * 36)));
                    tasks.Add(batch.StringSetAsync(string.Format(CacheKeys.Answer, item.Id), item.ToJsonStr(true), TimeSpan.FromSeconds(60 * 60 * 1)));
                },
                (item) => item.Id
            );

            // found
            var result = lsRes.DistinctBy(_ => _.Id).Select(_ => Convert_to_QaAnswerItemDto(_, userId)).ToList();
            lsCtn = lsCtn.DistinctBy(_ => _.Id).ToList();

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

            // ReplyCount
            if (result.Count > 0)
            {
                var rr = await GetAnswersReplyCounts(result.Select(_ => _.AnswerId));
                foreach (var dto in result)
                {
                    if (!rr.TryGetOne(out var x, _ => _.Item1 == dto.AnswerId)) continue;
                    dto.ReplyCount = x.Item2;
                }
            }

            // content
            if (result.Count > 0)
            {
                var notIn = result.Where(x => !lsCtn.Any(_ => _.Id == x.AnswerId)).Select(_ => _.AnswerId);
                if (notIn.Any())
                {
                    var rr = await _answerRepository.GetQaAnswersContent(notIn);
                    lsCtn.AddRange(rr);
                }
                foreach (var dto in result)
                {
                    if (!lsCtn.TryGetOne(out var x, _ => _.Item1 == dto.AnswerId)) continue;
                    dto.Content = x.Item2;
                }
            }

            // LikeCount IsLikeByMe
            for (var ___ = result.Count > 0; ___; ___ = false)
            {
                var ls = await GetAnswersLikeCounts(result.Select(_ => _.AnswerId), userId);
                foreach (var dto in result)
                {
                    if (!ls.TryGetOne(out var x, _ => _.Id == dto.AnswerId)) continue;
                    dto.LikeCount = x.LikeCount;
                    dto.IsLikeByMe = x.IsLikeByMe;
                }
            }

            return result;
        }

        static QaAnswerItemDto Convert_to_QaAnswerItemDto(QuestionAnswer s, Guid userId)
        {
            var t = new QaAnswerItemDto();
            t.AnswerId = s.Id;
            t.AnswerNo = UrlShortIdUtil.Long2Base32(s.No);
            t.UserId = s.UserId;
            t.CreateTime = s.CreateTime;
            t.EditTime = s.LastEditTime;
            t.Content = s.Content;
            t.Img = s.Imgs_s?.JsonStrTo<string[]>()?.FirstOrDefault() ?? s.Imgs?.JsonStrTo<string[]>()?.FirstOrDefault();
            t.LikeCount = s.LikeCount;
            t.ReplyCount = s.ReplyCount;
            t.IsMyAnswer = s.UserId == userId;
            t.IsAnony = s.IsAnony;
            if (t.IsAnony == true) t.UserName = s.AnonyUserName ?? $"匿名用户{t.AnswerId.ToString("n")[..7]}";
            else t.UserName = $"用户{t.AnswerId.ToString("n")[..7]}";
            t.UserHeadImg = BusinessLogicUtils.AnonyUserHeadImg;
            return t;
        }


        public async Task<IEnumerable<(Guid, int)>> GetAnswersReplyCounts(IEnumerable<Guid> ids)
        {
            ids = (ids ?? Enumerable.Empty<Guid>()).Distinct();
            List<(Guid, int)> result = new();

            // find in cache
            if (ids.Any())
            {
                var tasks = new List<Task<(Guid, int)>>();
                var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                foreach (var id in ids)
                {
                    var t = batch.StringGetAsync(string.Format(CacheKeys.AnswerCommentCount, id)).ContinueWith(t => (id, int.TryParse(t.Result, out var _i) ? _i : -1));
                    tasks.Add(t);
                }
                batch.Execute();
                await Task.WhenAll(tasks);

                result.AddRange(tasks.Select(_ => _.Result).Where(_ => _.Item2 != -1).ToList());

                ids = tasks.Select(_ => _.Result).Where(_ => _.Item2 == -1).Select(_ => _.Item1);
            }
            // find in db
            if (ids.Any())
            {
                var ls = await _answerRepository.GetAnswersReplyCounts(ids);
                result.AddRange(ls);

                var tasks = new List<Task>();
                var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                foreach (var dto in ls)
                {
                    var t = batch.StringSetAsync(string.Format(CacheKeys.AnswerCommentCount, dto.Item1), $"{dto.Item2}", TimeSpan.FromSeconds(60 * 20));
                    tasks.Add(t);
                }
                batch.Execute();
                await Task.WhenAll(tasks);
            }

            return result;
        }

        public async Task<IEnumerable<LikeCountDto>> GetAnswersLikeCounts(IEnumerable<Guid> ids, Guid userId = default)
        {
            var ids0 = ids;
            List<LikeCountDto> result = new();

            if (true)
            {
                ids = (ids0 ?? Enumerable.Empty<Guid>()).Distinct().ToArray();
                // find in cache
                if (ids.Any())
                {
                    var tasks = new List<Task<(Guid, int)>>();
                    var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                    foreach (var id in ids)
                    {
                        var t = batch.StringGetAsync(string.Format(CacheKeys.AnswerLikeCount, id)).ContinueWith(t => (id, int.TryParse(t.Result, out var _i) ? _i : -1));
                        tasks.Add(t);
                    }
                    batch.Execute();
                    await Task.WhenAll(tasks);

                    result.AddRange(tasks.Select(_ => _.Result).Where(_ => _.Item2 != -1).Select(_ => new LikeCountDto { Id = _.Item1, LikeCount = _.Item2 }));

                    ids = tasks.Select(_ => _.Result).Where(_ => _.Item2 == -1).Select(_ => _.Item1);
                }
                // find in db
                if (ids.Any())
                {
                    var ls = await _answerRepository.GetAnswersLikeCounts(ids);
                    result.AddRange(ls.Select(_ => new LikeCountDto { Id = _.Item1, LikeCount = _.Item2 }));

                    var tasks = new List<Task>();
                    var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                    foreach (var dto in ls)
                    {
                        var t = batch.StringSetAsync(string.Format(CacheKeys.AnswerLikeCount, dto.Item1), $"{dto.Item2}", TimeSpan.FromSeconds(60 * 20));
                        tasks.Add(t);
                    }
                    batch.Execute();
                    await Task.WhenAll(tasks);
                }
            }

            if (userId != default)
            {
                var lsRes = new List<(Guid, bool)>();
                ids = (ids0 ?? Enumerable.Empty<Guid>()).Distinct().ToArray();
                // find in cache
                if (ids.Any())
                {
                    var tasks = new List<Task<(Guid, int)>>();
                    var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                    foreach (var id in ids)
                    {
                        var t = batch.StringGetAsync(string.Format(CacheKeys.AnswerIsLikeByMe, id, userId)).ContinueWith(t => (id, int.TryParse(t.Result, out var _i) ? _i : -1));
                        tasks.Add(t);
                    }
                    batch.Execute();
                    await Task.WhenAll(tasks);

                    lsRes.AddRange(tasks.Select(_ => _.Result).Where(_ => _.Item2 != -1).Select(_ => (_.Item1, _.Item2 > 0)));

                    ids = tasks.Select(_ => _.Result).Where(_ => _.Item2 == -1).Select(_ => _.Item1);
                }
                // find in db
                if (ids.Any())
                {
                    var ls = await _userRepository.GetsIsLikeByMe(ids, userId, UserLikeType.Answer);                    
                    lsRes.AddRange(ls);

                    var tasks = new List<Task>();
                    var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                    foreach (var dto in ls)
                    {
                        var t = batch.StringSetAsync(string.Format(CacheKeys.AnswerIsLikeByMe, dto.Item1, userId), $"{dto.Item2}", TimeSpan.FromSeconds(60 * 20));
                        tasks.Add(t);
                    }
                    batch.Execute();
                    await Task.WhenAll(tasks);
                }

                foreach (var dto in result)
                {
                    if (!lsRes.TryGetOne(out var x, _ => _.Item1 == dto.Id)) continue;
                    dto.IsLikeByMe = x.Item2;
                }
            }

            return result;
        }


        public async Task<GetQuestionInviteUserLsQueryResult> GetQuesInviteUserList(GetQuestionInviteUserLsQuery query)
        {
            // 跟知乎一样, 同问题被邀请人只能被邀请一次，并且按钮会变成已邀请 不能再点了。

            var result = new GetQuestionInviteUserLsQueryResult();
            var _questionQuery = _services.GetService<IQuestionQuery>();

            var k = string.Format(CacheKeys.QuestionInviteUserLs, query.QuestionId);
            var lsUsers = await _easyRedisClient.GetAsync<List<GetQuestionInviteUserLsItemDto>>(k);
            if (lsUsers == null)
            {
                var question = (await _questionQuery.GetQuestionDbDtos(new[] { query.QuestionId })).FirstOrDefault();
                if (question == null) throw new ResponseResultException("问题不存在", Errcodes.Wenda_QuestionNotExists);
                
                var category = await _cityCategoryRepository.GetCategory(question.CategoryId ?? 0);
                if (category == null) throw new ResponseResultException("分类不存在", Errcodes.Wenda_CategoryNotExists);

                var cids = category.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var cidLv1 = int.TryParse(cids.ElementAtOrDefault(0), out var _c1) ? _c1 : -1;
                var cidLv2 = long.TryParse(cids.ElementAtOrDefault(1), out var _c2) ? _c2 : -1;
                if (cidLv1 == -1 || cidLv2 == -1) throw new ResponseResultException("分类不存在", 201);

                lsUsers = new List<GetQuestionInviteUserLsItemDto>();
                var talents = await _userApiService.GetTopNTalentUserByGrade(BusinessLogicUtils.Lv1CategoryIdToGrade(cidLv1), 20); // 达人s
                //
                // 同学段内部达人
                if (lsUsers.Count < 10)
                {                    
                    lsUsers.AddRange(talents.Where(_ => _.IsInternal).Take(10).Select(_ => new GetQuestionInviteUserLsItemDto(_)));
                }
                // 内部虚拟账号（随机展示）
                if (lsUsers.Count < 10)
                {
                    var rr = await _userApiService.GetTopNRandVirtualUser(10 - lsUsers.Count);
                    if (rr != null) lsUsers.AddRange(rr.Select(_ => new GetQuestionInviteUserLsItemDto(_)));
                }
                // 同学段的外部达人
                if (lsUsers.Count < 10)
                {                    
                    lsUsers.AddRange(talents.Where(_ => !_.IsInternal).Take(10 - lsUsers.Count).Select(_ => new GetQuestionInviteUserLsItemDto(_)));
                }
                // 关注同学段同领域的用户
                if (lsUsers.Count < 10)
                {
                    var userIds = await _userCategoryAttentionRepository.GetUserByAttentionSameCategory(cidLv2, 10 - lsUsers.Count);
                    var rr = await _userApiService.GetUsersDesc(userIds);
                    rr = rr.ItemsOrderBy(userIds, _ => _.Id);
                    lsUsers.AddRange(rr.Select(_ => new GetQuestionInviteUserLsItemDto(_)));
                }

                await _easyRedisClient.AddAsync(k, lsUsers, TimeSpan.FromSeconds(60 * 5));
            }

            // IsInvited
            if (lsUsers.Count > 0)
            {
                await foreach (var x in _answerRepository.GetUsersIsInvitedToQuesion(query.QuestionId, lsUsers.Select(_ => _.UserId)))
                {
                    if (!lsUsers.TryGetOne(out var dto, _ => _.UserId == x.UserId)) continue;
                    dto.IsInvited = x.IsInvited;
                }
            }

            result.Items = lsUsers;
            return result;
        }


        public async Task<Page<QaQuestionListItemDto>> GetMyAnswers(Guid userId, int pageIndex, int pageSize)
        { 
            var _questionQuery = _services.GetService<IQuestionQuery>();

            var pgIds = await _answerRepository.GetIdsByMyAnswers(userId, pageIndex, pageSize);

            var qids = pgIds.Data.Select(_ => _.Qid).Distinct().ToArray();
            var lsQ = await _questionQuery.GetQaItemDtos<QaQuestionListItemDto>(qids, userId: userId);
            //
            // 同一问题，如果用户回答了多次，是显示多条
            var lsQ2 = pgIds.Data.Select(x => 
            {
                var dto = lsQ.FirstOrDefault(_ => _.QuestionId == x.Qid);
                return (Copy(dto), x.Aid);
            }).ToArray();

            // answer
            var lsA = await GetQaAnswerItemDtos(ids: pgIds.Data.Select(_ => _.Aid), userId: userId);
            foreach (var (dto, aid) in lsQ2)
            {
                if (!lsA.TryGetOne(out var x, _ => _.AnswerId == aid)) continue;
                dto.Answer = x;
            }

            return new(lsQ2.Select(_ => _.Item1), pgIds.Total);
        }

        static QaQuestionListItemDto Copy(QaQuestionListItemDto s)
        {
            var str = s.ToJsonStr(true);
            var t = str.JsonStrTo<QaQuestionListItemDto>();
            return t;
        }
    }
}