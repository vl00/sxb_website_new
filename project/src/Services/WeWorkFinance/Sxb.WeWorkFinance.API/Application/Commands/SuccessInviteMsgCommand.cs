using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyWeChat.CustomMessage;

namespace Sxb.WeWorkFinance.API.Application.Commands
{
    public class SuccessInviteMsgCommand : IRequest<bool>
    {
        public string ToUser { get; private set; }
        public string UnionId { get; private set; }

        /// <summary>
        /// 是否主动发起
        /// </summary>
        public bool Initiative { get; private set; }

        public SuccessInviteMsgCommand(string toUser, string unionId,bool initiative = false)
        {
            this.UnionId = unionId;
            this.ToUser = toUser;
            this.Initiative = initiative;
        }
    }
}
