using MediatR;
using Polly;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Extensions;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.OtherAPIClient.Org;
using Sxb.WenDa.Common.OtherAPIClient.School;
using Sxb.WenDa.Common.OtherAPIClient.User;
using Sxb.WenDa.Query.SQL.Repositories;
using System.Diagnostics;

namespace Sxb.WenDa.API.Application.Commands
{
    public class AddQaCommentCommandHandler : IRequestHandler<AddQaCommentCommand, AddQaCommentCommandResult>
    {
        readonly ILogger log;
        readonly IAnswerRepository _answerRepository;
        readonly IQuestionRepository _questionRepository;
        readonly IEasyRedisClient _easyRedisClient;
        readonly IUserQuery _userQuery;        
        readonly ICommentRepository _commentRepository;
        readonly ILock1Factory _lock1Factory;

        public AddQaCommentCommandHandler(IAnswerRepository answerRepository, ILoggerFactory loggerFactory, ICommentRepository commentRepository,
            IUserQuery userQuery, IQuestionRepository questionRepository,
            IEasyRedisClient easyRedisClient)
        {
            log = loggerFactory.CreateLogger(this.GetType());
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
            _easyRedisClient = easyRedisClient;
            _userQuery = userQuery;            
            _commentRepository = commentRepository;
            _lock1Factory = new StecRedisLock1Factory1(_easyRedisClient);
        }

        public async Task<AddQaCommentCommandResult> Handle(AddQaCommentCommand cmd, CancellationToken cancellationToken)
        {
            var result = new AddQaCommentCommandResult();
            var now = DateTime.Now;
            cmd.AnswerId = cmd.AnswerId == Guid.Empty ? null : cmd.AnswerId;
            cmd.CommentId = cmd.CommentId == Guid.Empty ? null : cmd.CommentId;
            await default(ValueTask);

            if (cmd.CommentId != null && cmd.AnswerId != null)
                throw new ResponseResultException("answerId 与 commentId 只能传其中一个", 201);
            if (cmd.CommentId == null && cmd.AnswerId == null)
                throw new ResponseResultException("answerId 与 commentId 必须传其中一个", 201);
            if (string.IsNullOrEmpty(cmd.Content) || string.IsNullOrWhiteSpace(cmd.Content))
                throw new ResponseResultException("内容不能为空", 201);
            if (cmd.Content.Length > 200)
                throw new ResponseResultException("内容不能超过200字", 201);

            (QuestionAnswer answer, QaComment comment_from) = (default!, null);
            if (cmd.AnswerId != null)
            {
                answer = await _answerRepository.GetAnswer(cmd.AnswerId.Value);
            }
            if (cmd.CommentId != null)
            {
                comment_from = await _commentRepository.GetComment(cmd.CommentId.Value);
                if (comment_from?.IsValid == true)
                    answer = await _answerRepository.GetAnswer(comment_from.Aid);
            }
            if (cmd.CommentId != null && comment_from?.IsValid != true)
            {
                throw new ResponseResultException("评论不存在", Errcodes.Wenda_CommentNotExists);
            }
            if (answer?.IsValid != true)
            {
                throw new ResponseResultException("回答不存在", Errcodes.Wenda_AnswerNotExists);
            }

            // user
            var task_user = _userQuery.GetRealUser(cmd.UserId);

            var k = comment_from == null ? string.Format(CacheKeys.Wenda_lck_answer, cmd.AnswerId)
                : string.Format(CacheKeys.Wenda_lck_MainCommentId, comment_from.MainCommentId);
            await using var lck1 = await _lock1Factory.LockAsync(new Lock1Option(k).SetExpSec(60));
            if (!lck1.IsAvailable) throw new ResponseResultException("系统繁忙", Errcodes.Wenda_GetLck1Failed);

            // up ReplyCount
            if (comment_from != null)
            {
                comment_from.ReplyCount += 1;
                comment_from.Modifier = cmd.UserId;
                comment_from.ModifyDateTime = now;
            }
            else
            {
                answer.ReplyCount += 1;
                answer.Modifier = cmd.UserId;
                answer.ModifyDateTime = now;
            }

            var comment = new QaComment { Id = Guid.NewGuid(), IsValid = true };
            comment.Aid = answer.Id;
            comment.UserId = cmd.UserId;
            comment.CreateTime = now;
            comment.Modifier = comment.UserId;
            comment.ModifyDateTime = comment.CreateTime;
            comment.Content = cmd.Content;
            comment.IsAnony = false;
            comment.AnonyUserName = null;
            //
            // 一级 回复 回答
            // 其余级别 回复 评论
            comment.FromId = comment_from?.Id ?? null;
            comment.FromUserId = comment_from?.UserId ?? null;
            comment.MainCommentId = comment_from?.MainCommentId ?? comment.Id;
            comment.ReplyTotalCount = comment_from == null ? 0 : null;

            try
            {
                await task_user;
                await _commentRepository.AddComment(comment, comment_from, comment_from != null ? null : answer);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "add评论失败 {@cmd}", cmd);
                throw new ResponseResultException("系统繁忙", 201);
            }

            try
            {
                await Policy.Handle<Exception>()
                    .WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(600))
                    .ExecuteAsync(async () =>
                    {
                        var v = await _commentRepository.GetComment(comment.Id);
                        if (v == null) throw new Exception("write-read not sync");                        
                    });
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "added comment Id={comment_id}", comment.Id);
                throw new ResponseResultException($"系统繁忙:{Errcodes.Wenda_WriteReadNotSync}", Errcodes.Wenda_WriteReadNotSync);
            }

            if (cmd.UserId != default)
            {
                //var gzDto = await task_user;
                //result.HasGzWxGzh = gzDto?.HasGzWxGzh ?? false;
                //result.HasJoinWxEnt = gzDto?.HasJoinWxEnt ?? false;
            }

            // clear cache
            await _easyRedisClient.DelRedisKeys(new[]
            {
                string.Format(CacheKeys.Answer, answer.Id),
                string.Format(CacheKeys.AnswerCommentCount, answer.Id),
                //string.Format(CacheKeys.Answer, answer.Id) + ":*",
                //string.Format(CacheKeys.Question, answer.Qid),
                //CacheKeys.WendaAll,
            });

            result.CommentId = comment.Id;            
            return result;
        }
    }
}
