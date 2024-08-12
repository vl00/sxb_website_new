using StackExchange.Redis;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.OtherAPIClient.User;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.Repositories;

namespace Sxb.WenDa.API.Application.Query
{
    public class CommentQuery : ICommentQuery
    {
        readonly IAnswerRepository _answerRepository;
        readonly ICommentRepository _commentRepository;
        readonly IEasyRedisClient _easyRedisClient;
        readonly IUserApiService _userApiService;
        readonly IUserRepository _userRepository;

        public CommentQuery(IAnswerRepository answerRepository, ICommentRepository commentRepository,
            IUserApiService userApiService, IUserRepository userRepository,
            IEasyRedisClient easyRedisClient)
        { 
            _answerRepository = answerRepository;
            _easyRedisClient = easyRedisClient;
            _commentRepository = commentRepository;
            _userApiService = userApiService;
            _userRepository = userRepository;
        }

        public async Task<GetCommentsPageListQryResDto> GetCommentsPageList(GetCommentsPageListQuery query)
        {
            if (query.CommentId != null && query.AnswerId != null)
                throw new ResponseResultException("answerId 与 commentId 只能传其中一个", 201);
            if (query.CommentId == null && query.AnswerId == null)
                throw new ResponseResultException("answerId 与 commentId 必须传其中一个", 201);

            var now = DateTime.Now;
            var is_get1st = query.AnswerId != null; // 是否查一级评论

            var result = await _commentRepository.GetCommentsPageList(query);

            // children
            for (var ___ = result.Data.Any() && is_get1st; ___; ___ = false) // if-break
            {
                var rr = await _commentRepository.GetTop2ChildrenCommentsByMainCommentIds(result.Data.Select(_ => _.Id));
                foreach (var dto in result.Data)
                {
                    if (!rr.TryGetOne(out var x, _ => _.Item1 == dto.Id)) continue;
                    dto.Children = x.Item2.AsArray();
                }
            }
            // user
            for (var ___ = result.Data.Any(); ___; ___ = false) // if-break
            {
                var userIds = result.Data.Select(_ => _.UserId)
                    .Union(result.Data.Select(_ => _.FromUserId ?? default))
                    .Union(result.Data.Where(_ => _.Children?.Any() == true).SelectMany(_ => _.Children.Select(_ => _.UserId)))
                    .Union(result.Data.Where(_ => _.Children?.Any() == true).SelectMany(_ => _.Children.Select(_ => _.FromUserId ?? default)))
                    .Where(_ => _ != default)
                    .Distinct();

                if (!userIds.Any()) break;

                var rr = await _userApiService.GetUsersDesc(userIds);
                foreach (var dto in result.Data)
                {
                    if (!rr.TryGetOne(out var x, _ => _.Id == dto.UserId)) continue;
                    dto.UserName = x.Name;
                    dto.UserHeadImg = x.HeadImg;
                }
                foreach (var dto in result.Data)
                {
                    if (!rr.TryGetOne(out var x, _ => _.Id == dto.FromUserId)) continue;
                    dto.FromUserName = x.Name;
                }
                foreach (var dto1 in result.Data)
                {
                    if (dto1.Children == null) continue;
                    foreach (var dto in dto1.Children)
                    {
                        if (!rr.TryGetOne(out var x, _ => _.Id == dto.UserId)) continue;
                        dto.UserName = x.Name;
                        dto.UserHeadImg = x.HeadImg;
                    }
                    foreach (var dto in dto1.Children)
                    {
                        if (!rr.TryGetOne(out var x, _ => _.Id == dto.FromUserId)) continue;
                        dto.FromUserName = x.Name;
                    }
                }
            }

            // LikeCount IsLikeByMe
            for (var ___ = result.Data.Any(); ___; ___ = false) // if-break
            {
                var ids = result.Data.Select(_ => _.Id)
                    .Union(result.Data.Where(_ => _.Children != null).SelectMany(x => x.Children.Select(_ => _.Id)))
                    .Distinct();

                if (!ids.Any()) break;

                var rr = await GetCommentsLikeCounts(ids, query.UserId);
                foreach (var dto in result.Data)
                {
                    if (!rr.TryGetOne(out var x, _ => _.Id == dto.Id)) continue;
                    dto.LikeCount = x.LikeCount;
                    dto.IsLikeByMe = x.IsLikeByMe;
                }
                foreach (var dto1 in result.Data)
                {
                    if (dto1.Children == null) continue;
                    foreach (var dto in dto1.Children)
                    {
                        if (!rr.TryGetOne(out var x, _ => _.Id == dto.Id)) continue;
                        dto.LikeCount = x.LikeCount;
                        dto.IsLikeByMe = x.IsLikeByMe;
                    }
                }
            }
            
            return new GetCommentsPageListQryResDto { Data = result.Data, Total = result.Total, Time = query.Naf ?? now };
        }

        public async Task<IEnumerable<LikeCountDto>> GetCommentsLikeCounts(IEnumerable<Guid> ids, Guid userId = default)
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
                        var t = batch.StringGetAsync(string.Format(CacheKeys.CommentLikeCount, id)).ContinueWith(t => (id, int.TryParse(t.Result, out var _i) ? _i : -1));
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
                    var ls = await _commentRepository.GetCommentsLikeCounts(ids);
                    result.AddRange(ls.Select(_ => new LikeCountDto { Id = _.Item1, LikeCount = _.Item2 }));

                    var tasks = new List<Task>();
                    var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                    foreach (var dto in ls)
                    {
                        var t = batch.StringSetAsync(string.Format(CacheKeys.CommentLikeCount, dto.Item1), $"{dto.Item2}", TimeSpan.FromSeconds(60 * 20));
                        tasks.Add(t);
                    }
                    batch.Execute();
                    await Task.WhenAll(tasks);
                }
            }

            if (userId != default)
            {
                var ls_res = new List<(Guid, bool)>();
                ids = (ids0 ?? Enumerable.Empty<Guid>()).Distinct().ToArray();
                // find in cache
                if (ids.Any())
                {
                    var tasks = new List<Task<(Guid, int)>>();
                    var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                    foreach (var id in ids)
                    {
                        var t = batch.StringGetAsync(string.Format(CacheKeys.CommentIsLikeByMe, id, userId)).ContinueWith(t => (id, int.TryParse(t.Result, out var _i) ? _i : -1));
                        tasks.Add(t);
                    }
                    batch.Execute();
                    await Task.WhenAll(tasks);

                    ls_res.AddRange(tasks.Select(_ => _.Result).Where(_ => _.Item2 != -1).Select(_ => (_.Item1, _.Item2 > 0)));

                    ids = tasks.Select(_ => _.Result).Where(_ => _.Item2 == -1).Select(_ => _.Item1);
                }
                // find in db
                if (ids.Any())
                {
                    var ls = await _userRepository.GetsIsLikeByMe(ids, userId, UserLikeType.Comment);
                    ls_res.AddRange(ls);

                    var tasks = new List<Task>();
                    var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                    foreach (var dto in ls)
                    {
                        var t = batch.StringSetAsync(string.Format(CacheKeys.CommentIsLikeByMe, dto.Item1, userId), $"{dto.Item2}", TimeSpan.FromSeconds(60 * 20));
                        tasks.Add(t);
                    }
                    batch.Execute();
                    await Task.WhenAll(tasks);
                }

                foreach (var dto in result)
                {
                    if (!ls_res.TryGetOne(out var x, _ => _.Item1 == dto.Id)) continue;
                    dto.IsLikeByMe = x.Item2;
                }
            }

            return result;
        }

    }
}