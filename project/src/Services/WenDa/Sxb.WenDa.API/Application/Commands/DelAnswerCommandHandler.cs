using MediatR;
using Polly;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.OtherAPIClient.School;
using Sxb.WenDa.Common.OtherAPIClient.User;
using Sxb.WenDa.Query.SQL.Repositories;
using System.Diagnostics;

namespace Sxb.WenDa.API.Application.Commands
{
    public class DelAnswerCommandHandler : IRequestHandler<DelAnswerCommand, DelAnswerCommandResult>
    {
        readonly IAnswerRepository _answerRepository;
        readonly IQuestionRepository _questionRepository;
        readonly IEasyRedisClient _easyRedisClient;

        public DelAnswerCommandHandler(IAnswerRepository answerRepository, IQuestionRepository questionRepository,
            IEasyRedisClient easyRedisClient)
        {
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<DelAnswerCommandResult> Handle(DelAnswerCommand cmd, CancellationToken cancellationToken)
        {
            var result = new DelAnswerCommandResult();
            var now = DateTime.Now;
            await default(ValueTask);

            var answer = await _answerRepository.GetAnswer(cmd.AnswerId);
            if (answer?.IsValid != true) return result;
            if (answer.UserId != cmd.UserId) throw new ResponseResultException("不是我的回答", Errcodes.Wenda_IsNotMyAnswer);

            var question = await _questionRepository.GetQuestion(answer.Qid);
            if (question?.IsValid != true) throw new ResponseResultException("问题不存在", Errcodes.Wenda_QuestionNotExists);
            question.ReplyCount = Math.Max(0, question.ReplyCount - 1);
            question.ModifyDateTime = now;
            question.Modifier = cmd.UserId;

            answer.IsValid = false;
            answer.Modifier = cmd.UserId;
            answer.ModifyDateTime = now;

            result.Success = await _answerRepository.DeleteAnswer(answer, question);

            return result;
        }
    }
}
