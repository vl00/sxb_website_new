using DotNetCore.CAP;
using MediatR;
using Sxb.User.Domain.AggregatesModel.TalentAggregate;
using Sxb.User.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.Commands
{
    public class TalentApplyCommandHandle : IRequestHandler<TalentApplyCommand, Guid>
    {
        ITalentRepository _talentRepository;
        ICapPublisher _capPublisher;
        public TalentApplyCommandHandle(ITalentRepository talentRepository, ICapPublisher capPublisher)
        {
            _talentRepository = talentRepository;
            _capPublisher = capPublisher;
        }
        public async Task<Guid> Handle(TalentApplyCommand request, CancellationToken cancellationToken)
        {
            var talent = new Talent(Guid.NewGuid(),Guid.NewGuid(), 0, "123", "456");
            //talent.AddTalent(Guid.NewGuid(), 0, "123", "456");
            _talentRepository.Add(talent);
            await _talentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            return talent.Id;
        }
    }
}
