using System;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb
{
    public interface IArticleDynamicRepository
    {
        Task IncreaseViewCountAsync(string articleId);
    }
}