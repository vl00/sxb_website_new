using DotNetCore.CAP;
using MediatR;
using Sxb.User.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.Commands
{
    public class TalentInviteByCodeCommandHandle : IRequestHandler<TalentInviteByCodeCommand, long>
    {
        ITalentRepository _talentRepository;
        ICapPublisher _capPublisher;
        public TalentInviteByCodeCommandHandle(ITalentRepository talentRepository, ICapPublisher capPublisher)
        {
            _talentRepository = talentRepository;
            _capPublisher = capPublisher;
        }
        public Task<long> Handle(TalentInviteByCodeCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
