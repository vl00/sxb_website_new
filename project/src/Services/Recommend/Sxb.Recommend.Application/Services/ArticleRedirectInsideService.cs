using MediatR;
using MongoDB.Bson;
using Sxb.Framework.Foundation;
using Sxb.Infrastructure.Core.Extensions;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public class ArticleRedirectInsideService : IArticleRedirectInsideService
    {
        IMediator _mediator;
        IArticleRepository _articleRepository;
        IArticleRedirectInsideRepository _repository;

        public ArticleRedirectInsideService(IMediator mediator, IArticleRepository articleRepository, IArticleRedirectInsideRepository repository)
        {
            _mediator = mediator;
            _articleRepository = articleRepository;
            _repository = repository;
        }

        public async Task Add(ArticleRedirectInside articleRedirectInside)
        {
            await _repository.AddAsync(articleRedirectInside);
            articleRedirectInside.Create();
            await _mediator.DispatchDomainEventsAsync(articleRedirectInside);
        }

        public async Task Add(string shortNo1, string shortNo2)
        {
            var no1 = UrlShortIdUtil.Base322Long(shortNo1);
            var no2 = UrlShortIdUtil.Base322Long(shortNo2);
            var (id1, id2) = await _articleRepository.GetIdByNo(no1, no2);

            ArticleRedirectInside articleRedirectInside = new ArticleRedirectInside(id1, id2, DateTime.Now);
            await Add(articleRedirectInside);
        }


    }
}
