using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.Domain;
using Sxb.Infrastructure.Core.Extensions;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Event;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.DomainEventHandler
{
    public class SchoolRedirectInsideCreateEventHandler : IDomainEventHandler<SchoolRedirectInsideCreateEvent>
    {
        ILogger<SchoolRedirectInsideCreateEventHandler> _logger;
        ISchoolRedirectFrequencyRepository _schoolRedirectFrequencyRepository;
        ISchoolRedirectInsideRepository _schoolRedirectInsideRepository;
        IMediator _mediator;
        public SchoolRedirectInsideCreateEventHandler(ILogger<SchoolRedirectInsideCreateEventHandler> logger
            , ISchoolRedirectFrequencyRepository schoolRedirectFrequencyRepository
            , ISchoolRedirectInsideRepository schoolRedirectInsideRepository, IMediator mediator)
        {
            _logger = logger;
            _schoolRedirectFrequencyRepository = schoolRedirectFrequencyRepository;
            _schoolRedirectInsideRepository = schoolRedirectInsideRepository;
            _mediator = mediator;
        }

        public async Task Handle(SchoolRedirectInsideCreateEvent notification, CancellationToken cancellationToken)
        {
            //当产生一条跳转记录时，则更新对应的跳转数。
            var frequency = await _schoolRedirectInsideRepository.QueryFrequencyAsync(notification.SchoolRedirectInside.SIdP, notification.SchoolRedirectInside.SIdS);
            var existsFrequency = await _schoolRedirectFrequencyRepository.QueryFrequencyAsync(notification.SchoolRedirectInside.SIdP, notification.SchoolRedirectInside.SIdS);
            if (existsFrequency != null)
            {
                await _schoolRedirectFrequencyRepository.DeleteAsync(existsFrequency.Id);
            }
            await _schoolRedirectFrequencyRepository.AddAsync(frequency);
            if (existsFrequency?.OpenTime != frequency.OpenTime)
            {
                _logger.LogDebug($"检测到打开次数变更：{frequency.SIdP},{frequency.SIdS},{existsFrequency?.OpenTime}-->{frequency.OpenTime}");
                frequency.OpenTimeIsChange();
                await _mediator.DispatchDomainEventsAsync(frequency);
            }

        }
    }
}
