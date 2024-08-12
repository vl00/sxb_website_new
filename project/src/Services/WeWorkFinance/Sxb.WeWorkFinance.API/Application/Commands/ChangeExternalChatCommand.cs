using MediatR;
using SKIT.FlurlHttpClient.Wechat.Work.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Commands
{
    public class ChangeExternalChatCommand : IRequest<bool>
    {
        public string GroupChatId { get; private set; }

        public ChangeExternalChatCommand(string groupChatId)
        {
            this.GroupChatId = groupChatId;
        }
    }
}
