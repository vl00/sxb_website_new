using DotNetCore.CAP;
using Sxb.WenDa.API.Application.IntegrationEvents;
using Sxb.WenDa.Common.OtherAPIClient.User;
using Sxb.WenDa.Common.OtherAPIClient.User.Models;
using Sxb.WenDa.Query.SQL.QueryDto;
using Sxb.WenDa.Query.SQL.Repositories;

namespace Sxb.WenDa.API.Application.IntegrationEventHandlers
{
    public class NotifyIntegrationEventHandler : ICapSubscribe, INotifyIntegrationEventHandler
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IInvitationRepository _invitationRepository;

        private readonly ICapPublisher _capPublisher;
        private readonly IUserApiService _userApiService;

        public NotifyIntegrationEventHandler(IQuestionRepository questionRepository, ICommentRepository commentRepository, IInvitationRepository invitationRepository, IUserApiService userApiService, ICapPublisher capPublisher)
        {
            _questionRepository = questionRepository;
            _commentRepository = commentRepository;
            _invitationRepository = invitationRepository;
            _userApiService = userApiService;
            _capPublisher = capPublisher;
        }
        bool IsTest { get; set; }

        static readonly string NewAnswerCode = "WenDa:NewAnswerCode";
        static readonly string NewInvitation = "WenDa:NewInvitation";
        static readonly string NewComment = "WenDa:NewComment";

        [CapSubscribe(nameof(NotifyIntegrationEvent))]
        public async Task SendAsync(NotifyIntegrationEvent @event)
        {
            IsTest = @event.IsTest;

            var endTime = @event.Time;
            var startTime = endTime.AddDays(-1);

            await NotifyNewAnswerUser(endTime, startTime);
            await NotifyInvitationUser(endTime, startTime);
            await NotifyNewCommentUser(endTime, startTime);

        }

        /// <summary>
        /// 通知有新回答的用户
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private async Task NotifyNewAnswerUser(DateTime endTime, DateTime startTime)
        {
            int pageIndex = 1;
            int pageSize = int.MaxValue;//简单点

            var data = await _questionRepository.GetNewAnswerUserAsync(startTime, endTime, pageIndex, pageSize);
            await NotifyUser(NewAnswerCode, data);
        }

        /// <summary>
        /// 通知有被新邀请的用户
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private async Task NotifyInvitationUser(DateTime endTime, DateTime startTime)
        {
            int pageIndex = 1;
            int pageSize = int.MaxValue;//简单点

            var data = await _invitationRepository.GetInvitationToUserAsync(startTime, endTime, pageIndex, pageSize);
            await NotifyUser(NewInvitation, data);
        }

        /// <summary>
        /// 通知有新被评论的用户
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private async Task NotifyNewCommentUser(DateTime endTime, DateTime startTime)
        {
            int pageIndex = 1;
            int pageSize = int.MaxValue;//简单点

            var data = await _commentRepository.GetCommentFromUserAsync(startTime, endTime, pageIndex, pageSize);
            await NotifyUser(NewComment, data);
        }


        /// <summary>
        /// 使用指定消息模板发送消息
        /// </summary>
        /// <param name="templateSettingCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task NotifyUser(string templateSettingCode, IEnumerable<NotifyUserQueryDto> data)
        {
            var userIds = data.Select(s => s.UserId);
            if (!userIds.Any()) return;

            IEnumerable<UserWxFwhDto> users;
            if (IsTest)
            {
                var userId = Guid.Parse("FCBB302A-097D-4664-89E3-B659F8D62B92");
                data = new List<NotifyUserQueryDto>() { new NotifyUserQueryDto() { UserId = userId } };
                users = new List<UserWxFwhDto>() { new UserWxFwhDto() { UserId = userId, IsSubscribe = true, Nickname = "烦烦烦", OpenId = "oieSE6LfVn3z-jBEo0xcbYkcW-N8" } };
            }
            else
            {
                users = await _userApiService.GetWxNicknames(userIds);
            }

            //会执行两次Select
            //var tasks = users
            //    .Where(s => s.IsSubscribe && !string.IsNullOrWhiteSpace(s.OpenId))
            //    .Select(user =>
            //    {
            //        var item = data.FirstOrDefault(s => s.UserId == user.UserId);
            //        if (item == null) return null;

            //        return _capPublisher.PublishAsync(
            //            nameof(SendMsgIntegrationEvent),
            //            new SendMsgIntegrationEvent(templateSettingCode, user.OpenId, new Dictionary<string, string>()
            //            {
            //                { "Nickname", user.Nickname },
            //                { "QuestionNo", item.QuestionNo },
            //                { "AnswerNo", item.AnswerNo },
            //            })
            //        );
            //    })
            //    .Where(t => t != null);

            var tasks = new List<Task>();
            foreach (var user in users)
            {
                if (!user.IsSubscribe || string.IsNullOrWhiteSpace(user.OpenId)) continue;

                var item = data.FirstOrDefault(s => s.UserId == user.UserId);
                if (item == null) continue;

                //publish
                var task = _capPublisher.PublishAsync(
                    nameof(SendMsgIntegrationEvent),
                    new SendMsgIntegrationEvent(templateSettingCode, user.OpenId, new Dictionary<string, string>()
                    {
                            { "Nickname", user.Nickname },
                            { "QuestionNo", item.QuestionNo },
                            { "AnswerNo", item.AnswerNo },
                    })
                );
                tasks.Add(task);
            }

            if (tasks.Any())
                Task.WaitAll(tasks.ToArray());
        }
    }
}
