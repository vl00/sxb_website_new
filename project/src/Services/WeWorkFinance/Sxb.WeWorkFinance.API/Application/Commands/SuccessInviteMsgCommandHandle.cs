using MediatR;
using Sxb.Framework.Cache.Redis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sxb.Framework.Foundation;
using System.Text;
using EasyWeChat.Interface;
using EasyWeChat.CustomMessage;
using Sxb.WeWorkFinance.API.Application.HttpClients;
using Sxb.WeWorkFinance.API.Config;
using Microsoft.Extensions.Options;
using Sxb.WeWorkFinance.Infrastructure.Repositories;
using Sxb.WeWorkFinance.API.Application.Queries;

namespace Sxb.WeWorkFinance.API.Application.Commands
{
    public class SuccessInviteMsgCommandHandle : IRequestHandler<SuccessInviteMsgCommand, bool>
    {
        private readonly ICustomMessageService _customMessageService;
        private readonly ITemplateMessageService _templateMessageService;
        private readonly IEasyRedisClient _easyRedisClient;

        private readonly IContactRepository _contactRepository;
        private readonly IWeixinQueries _weixinQueries;
        private readonly WxServerClient _wxClient;
        private readonly OrgServerClient _orgClient;
        private readonly InviteActivityConfig _inviteActivityConfig;

        public SuccessInviteMsgCommandHandle(IEasyRedisClient easyRedisClient, IContactRepository contactRepository,
            ICustomMessageService customMessageService, ITemplateMessageService templateMessageService, IWeixinQueries weixinQueries,
            WxServerClient wxClient, OrgServerClient orgClient, IOptions<InviteActivityConfig> inviteActivityConfig)
        {
            _contactRepository = contactRepository;
            _customMessageService = customMessageService;
            _templateMessageService = templateMessageService;
            _weixinQueries = weixinQueries;
            _easyRedisClient = easyRedisClient;
            _wxClient = wxClient;
            _orgClient = orgClient;
            _inviteActivityConfig = inviteActivityConfig.Value;
        }
        public async Task<bool> Handle(SuccessInviteMsgCommand request, CancellationToken cancellationToken)
        {
            //公众号向发展人发送消息
            string inviterKey = "org:InviteActivity:InviterBonusPoint";
            int inviteTotal = _contactRepository.GetCount(q => q.ParentUserid == request.UnionId && q.IsJoinGroup == true && q.Gender == 2 && q.IsLastActivity == false);
            int bonusPoint = (int)(await _easyRedisClient.SortedSetScoreAsync(inviterKey, request.UnionId ) ?? 0);

            var customerDetail = await _weixinQueries.GetCustomerDetail(request.UnionId);
            var cityCode = customerDetail?.City.ToString();

            //可兑换商品列表
            var activityCourses = await _orgClient.GetInviteActivityCourses(cityCode);
            var courses = activityCourses.Where(q => q.ExchangeType == 2 && q.ExchangePoint <= bonusPoint).OrderByDescending(o => o.ExchangePoint).ToList();

            //最低兑换积分的商品
            int minExchangePoint = activityCourses.Any() ? activityCourses.Where(q => q.ExchangeType == 2).Min(q => q.ExchangePoint) : 999999;

            string messageText = "";
            string messageTextTemp = "";

            if (inviteTotal < minExchangePoint)//邀请总人数不足最低兑换积分
            {
                messageTextTemp = _inviteActivityConfig.FirstSuccessInviteMessage;
                messageText = string.Format(messageTextTemp, bonusPoint);
            }
            else
            {
                if (bonusPoint >= minExchangePoint)//可用积分
                {
                    //发送间隔。积分>=10的时候，每5积分发送一次；积分>=30的时候，每10积分发送一次
                    int sendInterval = 1;
                    if (bonusPoint >= 10 && bonusPoint < 30)
                    {
                        sendInterval = 5;
                    }
                    else if(bonusPoint >= 30)
                    {
                        sendInterval = 10;
                    }
                    if ((bonusPoint % sendInterval != 0) && !request.Initiative)
                    {
                        return true;//不发送了
                    }
                    //间隔提醒文本
                    string sendIntervalMsg = sendInterval == 1 ? "" : string.Format("\n\n后续增加积分将{0}积分推送一次，如需查看进度回复【积分】即可查询当前积分~[加油]", sendInterval);

                    StringBuilder shopTitle = new();
                    foreach (var course in courses)
                    {
                        shopTitle.Append("" + course.Title + " 关键词：【" + string.Join("、", course.ExchangeKeywords) + "】。\n");
                    }

                    if (inviteTotal == bonusPoint)//邀请人数和可用积分一致，说明用户还没有兑换过积分
                    {
                        messageTextTemp = _inviteActivityConfig.SuccessInviteMessage;
                        messageText = string.Format(messageTextTemp + sendIntervalMsg, inviteTotal, bonusPoint, shopTitle.ToString());
                    }
                    else
                    {
                        messageTextTemp = _inviteActivityConfig.PointRewardAfterMessage;
                        messageText = string.Format(messageTextTemp + sendIntervalMsg, inviteTotal, inviteTotal - bonusPoint, bonusPoint, shopTitle.ToString());
                    }
                }
                else
                {
                    //积分不足最低兑换积分
                    messageTextTemp = _inviteActivityConfig.PointLackMessage;
                    messageText = string.Format(messageTextTemp, inviteTotal, inviteTotal - bonusPoint, bonusPoint);
                }
            }

            var message = new TextMessage();
            message.SetContent(messageText);

            var fwhAccessToken = await _wxClient.GetAccessToken();
            var result = await _customMessageService.SendAsync(fwhAccessToken, request.ToUser, message) ;

            if (!result.IsSuccess)
            {
                //发送不成功，尝试使用模板消息再推一次
                var templateMsg = new EasyWeChat.Model.SendTemplateRequest(request.ToUser, _inviteActivityConfig.TemplateMsgId);
                templateMsg.SetData(new EasyWeChat.Model.TemplateDataFiled[4] {
                    new EasyWeChat.Model.TemplateDataFiled {Filed = "first",Value =string.Format("已获得{0}积分！请回复关键词【积分】查询当前积分进度 ！", inviteTotal)},
                    new EasyWeChat.Model.TemplateDataFiled {Filed = "keyword1",Value ="【双十一！1元囤好礼】活动" },
                    new EasyWeChat.Model.TemplateDataFiled {Filed = "keyword2",Value =string.Format("当前积分为{0}分。", bonusPoint) },
                    new EasyWeChat.Model.TemplateDataFiled {Filed = "remark",Value ="请回复关键词【积分】获取当前奖品进度"},
                });
                await _templateMessageService.SendAsync(fwhAccessToken, templateMsg);
            }
            return result.IsSuccess;
        }


        
    }
}
