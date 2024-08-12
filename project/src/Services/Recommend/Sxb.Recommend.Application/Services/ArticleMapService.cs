using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public class ArticleMapService : IArticleMapService
    {
        IComputeScoreService _computeScoreService;
        IArticleMapRepository _articleMapRepository;
        IArticleFileRepository _articleFileRepository;
        public ArticleMapService(IArticleMapRepository articleMapRepository
            , IComputeScoreService computeScoreService
            , IArticleFileRepository articleFileRepository)
        {
            _articleMapRepository = articleMapRepository;
            _computeScoreService = computeScoreService;
            _articleFileRepository = articleFileRepository;
        }

        public async  Task ClearAll()
        {
            await _articleMapRepository.ClearAll();
        }

        public async Task<IEnumerable<ArticleMap>> UpsertArticelMaps(Article article)
        {
            var articelMaps = await _computeScoreService.GetArticleMaps(article);
            await _articleMapRepository.InsertManyAsync(articelMaps);
            return articelMaps;
        }

        public async Task UpsertArticelMaps(IEnumerable<Guid> articleIds)
        {
            foreach (var articleId in articleIds)
            {
                var article = _articleFileRepository.Query(s => s.IsOnline && s.Id == articleId).FirstOrDefault();
                if (article != null)
                {
                    await UpsertArticelMaps(article);
                }

            }
        }
    }
}
