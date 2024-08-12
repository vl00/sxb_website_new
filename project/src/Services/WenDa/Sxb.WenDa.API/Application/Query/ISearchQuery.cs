using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;

namespace Sxb.WenDa.API.Application.Query
{
    public interface ISearchQuery
    {
        Task<Page<SearchAggregate>> AggregateSearchAsync(AggregateQueryModel queryModel);
    }
}