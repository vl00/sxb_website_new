using MediatR;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sxb.Infrastructure.Core.Extensions;
using Sxb.Recommend.Domain.Entity;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace Sxb.Recommend.Application.Services
{
    public class ArticleRedirectFrequencyService : IArticleRedirectFrequencyService
    {
        IArticleRedirectFrequencyRepository _repository;
        IMediator _mediator;
        ILogger<ArticleRedirectFrequencyService> _logger;
        IHostEnvironment _environment;
        IMemoryCache _memoryCache;
        public ArticleRedirectFrequencyService(IArticleRedirectFrequencyRepository repository
            , IMediator mediator
            , ILogger<ArticleRedirectFrequencyService> logger
            , IHostEnvironment environment
            , IMemoryCache memoryCache)
        {
            _repository = repository;
            _mediator = mediator;
            _logger = logger;
            _environment = environment;
            _memoryCache = memoryCache;
        }



        /// <summary>
        /// 获取频率
        /// 针对aidp的频率做了动态缓存，优化查询效率。
        /// </summary>
        /// <param name="aidp"></param>
        /// <param name="aids"></param>
        /// <returns></returns>
        public async Task<ArticleRedirectFrequency> GetFrequency(Guid aidp, Guid aids)
        {

            var data = await _memoryCache.GetOrCreateAsync($"frequency_{aidp}", async (entry) =>
            {
                entry.SetSize(0).SetSlidingExpiration(TimeSpan.FromSeconds(5));
                return await _repository.QueryFrequenciesAsync(aidp);
            });
            return data.FirstOrDefault(s => s.AIdS == aids);
        }

        public async Task<IEnumerable<ArticleRedirectFrequency>> GetFrequenciesAsync(Guid aidp)
        {

            var data = await _memoryCache.GetOrCreateAsync($"frequency_{aidp}", async (entry) =>
            {
                entry.SetSize(0).SetSlidingExpiration(TimeSpan.FromSeconds(5));
                return await _repository.QueryFrequenciesAsync(aidp);
            });
            return data;
        }
    }
}
