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
    public class AddAnswerCommandHandler : IRequestHandler<AddAnswerCommand, AddAnswerCommandResult>
    {
        readonly ILogger log;
        readonly IAnswerRepository _answerRepository;
        readonly IQuestionRepository _questionRepository;
        readonly IEasyRedisClient _easyRedisClient;
        readonly IUserQuery _userQuery;

        public AddAnswerCommandHandler(IAnswerRepository answerRepository, ILoggerFactory loggerFactory,
            IUserQuery userQuery, IQuestionRepository questionRepository,
            IEasyRedisClient easyRedisClient)
        {
            log = loggerFactory.CreateLogger(this.GetType());
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
            _easyRedisClient = easyRedisClient;
            _userQuery = userQuery;            
        }

        public async Task<AddAnswerCommandResult> Handle(AddAnswerCommand cmd, CancellationToken cancellationToken)
        {
            var result = new AddAnswerCommandResult();
            var now = DateTime.Now;
            await default(ValueTask);

            if (string.IsNullOrEmpty(cmd.Content) || string.IsNullOrWhiteSpace(cmd.Content)) 
                throw new ResponseResultException("内容不能为空", 201);
            if ((cmd.Imgs?.Length ?? 0) != (cmd.Imgs_s?.Length ?? 0)) 
                throw new ResponseResultException("图片参数错误", 201);

            var question = await _questionRepository.GetQuestion(cmd.QuestionId);
            if (question?.IsValid != true) throw new ResponseResultException("问题不存在", Errcodes.Wenda_QuestionNotExists);
            question.ReplyCount += 1;
            question.ModifyDateTime = now;
            question.Modifier = cmd.UserId;

            // 需要判断我是否是被邀请回答的            
            // 被邀请的人可以多个, 并且他们可以匿名回答
            var invitation = await _answerRepository.GetInvitation(cmd.QuestionId, cmd.UserId);
            if (invitation?.IsValid != true) invitation = null;
            if (invitation != null)
            {
                invitation.AnswerTime = now;
            }

            //
            // check ok
            //

            var task_user = _userQuery.GetRealUser(cmd.UserId);

            var answer = new QuestionAnswer { Id = Guid.NewGuid(), IsValid = true };
            answer.Qid = cmd.QuestionId;
            answer.UserId = cmd.UserId;
            answer.CreateTime = now;
            answer.Modifier = cmd.UserId;
            answer.ModifyDateTime = answer.CreateTime;
            answer.Imgs = cmd.Imgs.ToJson();
            answer.Imgs_s = cmd.Imgs_s.ToJson();
            answer.Content = cmd.Content;
            answer.IsAnony = cmd.IsAnony;
            answer.AnonyUserName = !cmd.IsAnony ? null : BusinessLogicUtils.RandomAnonyUserName();

            try
            {
                await task_user;
                await _answerRepository.AddAnswer(answer, question, invitation);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "add回答失败 {@cmd}", cmd);
                throw new ResponseResultException("系统繁忙", 201);
            }

            try
            {
                await Policy.Handle<Exception>()
                    .WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(600))
                    .ExecuteAsync(async () =>
                    {
                        var a1 = await _answerRepository.GetAnswer(answer.Id);
                        if (a1 == null) throw new Exception("write-read not sync");
                        //answer.No = a1.No;
                    });
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "added answer Id={aid}", answer.Id);
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
                string.Format(CacheKeys.Question, answer.Qid),
                string.Format(CacheKeys.MyAnswerCount, cmd.UserId),
                string.Format(CacheKeys.Question, answer.Qid) + ":*",
                $"wenda:answers:qid_{answer.Qid}:*",
                CacheKeys.QuestionAnswersPageListAll,
                //CacheKeys.WendaAll,
            });

            result.AnswerId = answer.Id;
            result.AnswerNo = UrlShortIdUtil.Long2Base32(answer.No);
            return result;
        }
    }
}
