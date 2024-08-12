using MediatR;
using SKIT.FlurlHttpClient.Wechat.Work.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Commands
{
    public class ChangeExternalContactCommand : IRequest<bool>
    {
        public ChangeExternalContactEvent Event { get; private set; }

        public ChangeExternalContactCommand(ChangeExternalContactEvent @event)
        {
            this.Event = @event;
        }
    }
}
