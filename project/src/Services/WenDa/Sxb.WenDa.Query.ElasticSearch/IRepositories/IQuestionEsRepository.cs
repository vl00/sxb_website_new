using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;

namespace Sxb.WenDa.Query.ElasticSearch
{
    public interface IQuestionEsRepository : IBaseEsRepository<SearchQuestion>
    {
        void AddTest();
        Task<(long total, List<SearchAggregate> data)> AggregateSearchAsync(AggregateQueryModel queryModel);
    }
}