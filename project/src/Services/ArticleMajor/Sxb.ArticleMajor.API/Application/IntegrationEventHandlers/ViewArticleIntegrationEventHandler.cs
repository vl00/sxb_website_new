using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.ArticleMajor.Query.Mongodb;
using Sxb.Framework.Foundation;

namespace Sxb.ArticleMajor.API.Application.IntegrationEvents
{
    public class ViewArticleIntegrationEventHandler : ICapSubscribe, IViewArticleIntegrationEventHandler
    {
        private readonly ILogger<ViewArticleIntegrationEventHandler> _logger;
        private readonly IArticleDynamicRepository _articleDynamicRepository;

        public ViewArticleIntegrationEventHandler(ILogger<ViewArticleIntegrationEventHandler> logger, IArticleDynamicRepository articleDynamicRepository)
        {
            _logger = logger;
            _articleDynamicRepository = articleDynamicRepository;
        }

        [CapSubscribe(nameof(ViewArticleIntegrationEvent))]
        public async Task Handle(ViewArticleIntegrationEvent evt)
        {
            if (evt == null)
            {
                _logger.LogError("ViewArticle,接收的数据为空");
                return;
            }
            _logger.LogInformation("ViewArticle,data={0}", evt.ToJson());

            //check NonceStr

            await _articleDynamicRepository.IncreaseViewCountAsync(evt.Id);
        }
    }
}