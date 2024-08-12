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
using Sxb.WenDa.Query.SQL.Repositories;
using System.Diagnostics;

namespace Sxb.WenDa.API.Application.Commands
{
    public class EditAnswerCommandHandler : IRequestHandler<EditAnswerCommand, EditAnswerCommandResult>
    {
        readonly ILogger log;
        readonly IAnswerRepository _answerRepository;
        readonly IEasyRedisClient _easyRedisClient;
        readonly IUserQuery _userQuery;        

        public EditAnswerCommandHandler(IAnswerRepository answerRepository, ILoggerFactory loggerFactory,
            IUserQuery userQuery,
            IEasyRedisClient easyRedisClient)
        {
            log = loggerFactory.CreateLogger(this.GetType());
            _answerRepository = answerRepository;
            _easyRedisClient = easyRedisClient;
            _userQuery = userQuery;            
        }

        public async Task<EditAnswerCommandResult> Handle(EditAnswerCommand cmd, CancellationToken cancellationToken)
        {
            var result = new EditAnswerCommandResult();
            await default(ValueTask);

            var answer = await _answerRepository.GetAnswer(cmd.AnswerId);
            if (answer?.IsValid != true) throw new ResponseResultException("回答不存在", Errcodes.Wenda_AnswerNotExists);
            if (answer.UserId != cmd.UserId) throw new ResponseResultException("不是我的回答", Errcodes.Wenda_IsNotMyAnswer);

            var task_user = _userQuery.GetRealUser(cmd.UserId);

            answer.EditCount += 1;
            answer.LastEditTime = DateTime.Now;
            answer.Modifier = cmd.UserId;
            answer.ModifyDateTime = answer.LastEditTime;
            answer.Imgs = cmd.Imgs.ToJson();
            answer.Imgs_s = cmd.Imgs_s.ToJson();
            answer.Content = cmd.Content;
            answer.IsAnony = cmd.IsAnony;
            //answer.AnonyUserName = !cmd.IsAnony ? null : AnonyUserUtils.RndName();

            try
            {
                await _answerRepository.EditAnswer(answer);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "编辑回答失败 {@cmd}", cmd);
                throw new ResponseResultException("系统繁忙", 201);
            }

            if (cmd.UserId != default)
            {
                var gzDto = await task_user;
                //result.HasGzWxGzh = gzDto?.HasGzWxGzh ?? false;
                //result.HasJoinWxEnt = gzDto?.HasJoinWxEnt ?? false;
            }

            // clear cache
            await _easyRedisClient.DelRedisKeys(new[]
            {
                string.Format(CacheKeys.Answer, answer.Id),
                string.Format(CacheKeys.Question, answer.Qid),
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
