using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sxb.Domain;
using Sxb.Recommend.Application.Services;
using Sxb.Recommend.Domain.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.DomainEventHandler
{
    public class ArticleOpenTimeIsChangeEventHandler : IDomainEventHandler<ArticleOpenTimeIsChangeEvent>
    {
        ILogger<ArticleOpenTimeIsChangeEventHandler> _logger;
        IRecommendService _recommendService;
        IServiceProvider _serviceProvider;
        public ArticleOpenTimeIsChangeEventHandler(ILogger<ArticleOpenTimeIsChangeEventHandler> logger
            , IRecommendService recommendService
            , IServiceProvider serviceProvider)
        {
            _logger = logger;
            _recommendService = recommendService;
            _serviceProvider = serviceProvider;
        }

        public Task Handle(ArticleOpenTimeIsChangeEvent notification, CancellationToken cancellationToken)
        {
            //当跳转次数变更时，对应的学校匹配分数也需要刷新。
            var article = _recommendService.QueryArticles(s => s.Id == notification.ArticleRedirectFrequency.AIdP).FirstOrDefault();
            //异步后台更新学校匹配结果
            _ = Task.Run(async () =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    IArticleMapService articleMapService = scope.ServiceProvider.GetService<IArticleMapService>();
                    await articleMapService.UpsertArticelMaps(article);
                }

            });
            return Task.CompletedTask;

        }
    }
}
