using MediatR;
using Polly;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
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
    public class InviteUserToAnswerQuestionCommandHandler : IRequestHandler<InviteUserToAnswerQuestionCommand, InviteUserToAnswerQuestionCommandResult>
    {
        readonly IQuestionRepository _questionRepository;
        readonly IAnswerRepository _answerRepository;
        readonly ILogger log;
        readonly IUserApiService _userApiService;

        public InviteUserToAnswerQuestionCommandHandler(IQuestionRepository questionRepository, IAnswerRepository answerRepository, 
            ILoggerFactory loggerFactory, IUserApiService userApiService, 
            IServiceProvider services)
        {
            this.log = loggerFactory.CreateLogger(this.GetType());
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userApiService = userApiService;
        }

        public async Task<InviteUserToAnswerQuestionCommandResult> Handle(InviteUserToAnswerQuestionCommand cmd, CancellationToken cancellationToken)
        {
            // 可以不是问题作者的用户邀请别人回答
            // 跟知乎一样, 同问题被邀请人只能被邀请一次，并且按钮会变成已邀请 不能再点了。

            if (cmd.UserId == default) throw new ResponseResultException("未登录", 401);
            if (cmd.ToUserId == default) throw new ResponseResultException("邀请人为空不存在", 201);

            var result = new InviteUserToAnswerQuestionCommandResult();
            await default(ValueTask);

            var question = await _questionRepository.GetQuestion(cmd.QuestionId);
            if (question?.IsValid != true) throw new ResponseResultException("问题不存在", Errcodes.Wenda_QuestionNotExists);
            if (cmd.UserId == cmd.ToUserId) throw new ResponseResultException("自己不能邀请自己", 201);
            //if (question.ReplyCount > 0) throw new ResponseResultException("问题已有回答了", 201);

            var invitation = await _answerRepository.GetInvitation(cmd.QuestionId, cmd.ToUserId);
            if (invitation != null) return result;

            invitation = new Invitation { Id = Guid.NewGuid() };
            invitation.Qid = cmd.QuestionId;
            invitation.FromUserId = cmd.UserId;
            invitation.ToUserId = cmd.ToUserId;
            invitation.InviteTime = DateTime.Now;
            await _answerRepository.AddInvitation(invitation);            
            result.Success = true;
            
            return result;
        }
    }
}
