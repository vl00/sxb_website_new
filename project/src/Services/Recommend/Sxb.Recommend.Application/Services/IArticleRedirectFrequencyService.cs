using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public interface IArticleRedirectFrequencyService
    {


        Task<ArticleRedirectFrequency> GetFrequency(Guid aidp, Guid aids);
        Task<IEnumerable<ArticleRedirectFrequency>> GetFrequenciesAsync(Guid aidp);
    }
}
