using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
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
    public class ArticleRedirectInsideCreateEventHandler : IDomainEventHandler<ArticleRedirectInsideCreateEvent>
    {
        ILogger<ArticleRedirectInsideCreateEventHandler> _logger;
        IMediator _mediator;
        IArticleRedirectInsideRepository _articleRedirectInsideRepository;
        IArticleRedirectFrequencyRepository _articleRedirectFrequencyRepository;
        public ArticleRedirectInsideCreateEventHandler(ILogger<ArticleRedirectInsideCreateEventHandler> logger
            , IMediator mediator
            , IArticleRedirectInsideRepository articleRedirectInsideRepository
            , IArticleRedirectFrequencyRepository articleRedirectFrequencyRepository)
        {
            _logger = logger;
            _mediator = mediator;
            _articleRedirectInsideRepository = articleRedirectInsideRepository;
            _articleRedirectFrequencyRepository = articleRedirectFrequencyRepository;
        }

        public async Task Handle(ArticleRedirectInsideCreateEvent notification, CancellationToken cancellationToken)
        {
            //当产生一条跳转记录时，则更新对应的打开次数数。
            var frequencyVal = await _articleRedirectInsideRepository.StaticFrequencyAsync(notification.ArticleRedirectInside.AIdP, notification.ArticleRedirectInside.AIdS);
            var frequency = await _articleRedirectFrequencyRepository.GetOrCreate(notification.ArticleRedirectInside.AIdP, notification.ArticleRedirectInside.AIdS);
            bool updateFlag = frequency.UpdateOpenTimes(frequencyVal.OpenTimes);
            if (updateFlag)
            {
                _logger.LogDebug($"{frequency.AIdP}->{frequency.AIdS}的跳转次数发生变更。目前跳转数为{frequency.OpenTimes}");
            }
            await _articleRedirectFrequencyRepository.UpsertFrequency(frequency);
            await _mediator.DispatchDomainEventsAsync(frequency);

        }
    }
}
