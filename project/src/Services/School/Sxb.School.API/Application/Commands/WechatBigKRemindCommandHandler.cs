using DotNetCore.CAP;
using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.School.API.Application.IntegrationEvents;
using Sxb.School.Common.Entity;
using Sxb.School.Common.OtherAPIClient.User;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sxb.Framework.Foundation;

namespace Sxb.School.API.Application.Commands
{
    public class WechatBigKRemindCommandHandler : IRequestHandler<WechatBigKRemindCommand, ResponseResult>
    {
        const string BigKStartRemindMessage = "BigKStartRemindMessage";
        const string BigKEndRemindMessage = "BigKEndRemindMessage";
        const string SubscribeGroupCode = "BigK";


        private readonly ICapPublisher _capPublisher;

        private readonly IUserAPIClient _userAPIClient;

        private readonly IWechatBigKRepository _wechatBigKRepository;

        public WechatBigKRemindCommandHandler(ICapPublisher capPublisher, IUserAPIClient userAPIClient, IWechatBigKRepository wechatBigKRepository)
        {
            _capPublisher = capPublisher;
            _userAPIClient = userAPIClient;
            _wechatBigKRepository = wechatBigKRepository;
        }


        /// <summary>
        /// 事项开始的前一天晚上22点
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<ResponseResult> Handle(WechatBigKRemindCommand request, CancellationToken cancellationToken)
        {
            //明天仍在有效期内的活动日程
            var date = DateTime.Today.AddDays(1);
            var schedules = await _wechatBigKRepository.GetAvaliableSchedulesAsync(date);
            //schedules = GetTestData(date);

            foreach (var schedule in schedules)
            {
                var items = schedule.DataJson_Obj;
                //无日程列表
                if (items == null || !items.Any())
                {
                    continue;
                }

                foreach (var item in items)
                {

                    var extraData = CommonHelper.ToDictionary(new
                    {
                        item.Content,
                        StartDate = item.StartDate.ToString("yyyy-MM-dd"),
                        EndDate = item.EndDate.ToString("yyyy-MM-dd"),
                    });

                    //开始时间不是当天
                    if (item.StartDate == date)
                    {
                        //发送日程事项开始提醒
                        await SendRemindAsync(BigKStartRemindMessage, schedule.ID, extraData);
                    }
                    //开始和结束在同一天, 则只发开始提醒
                    else if (item.EndDate == date)
                    {
                        //发送日程事项结束提醒
                        await SendRemindAsync(BigKEndRemindMessage, schedule.ID, extraData);
                    }
                }
            }
            return ResponseResult.Success();
        }

        /// <summary>
        /// 发送提醒
        /// </summary>
        /// <param name="msgSettingCode"></param>
        /// <param name="subjectId"></param>
        /// <param name="extraData"></param>
        /// <returns></returns>
        public async Task SendRemindAsync(string msgSettingCode, Guid subjectId, Dictionary<string, string> extraData)
        {
            int pageIndex = 1;
            int pageSize = int.MaxValue;
            //get users
            var userIds = await _userAPIClient.GetSubscribeRemindsUserIdsAsync(SubscribeGroupCode, subjectId, pageIndex, pageSize);
            foreach (var userId in userIds)
            {
                _capPublisher.Publish(nameof(SendMsgIntegrationEvent), new SendMsgIntegrationEvent(msgSettingCode, userId, extraData));
            }
        }












        /// <summary>
        /// 测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static IEnumerable<WeChatRecruitScheduleInfo> GetTestData(DateTime date)
        {
            return new List<WeChatRecruitScheduleInfo>() {
                new WeChatRecruitScheduleInfo()
                {
                    ID = Guid.Parse("{FCBB302A-097D-4664-89E3-B659F8D62B92}"),
                    DataJson = new List<WeChatRecruitScheduleItem>()
                    {
                        new WeChatRecruitScheduleItem()
                        {
                            StartDate = date,
                            EndDate = date,
                            Content = "这是一个测试日程"
                        }
                    }.ToJson()
                }
            };
        }

    }
}
