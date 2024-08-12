using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.ElasticSearch;
using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;

namespace Sxb.WenDa.API.Application.Query
{
    public class SearchQuery : ISearchQuery
    {
        private readonly IQuestionEsRepository _questionEsRepository;

        public SearchQuery(IQuestionEsRepository questionEsRepository)
        {
            _questionEsRepository = questionEsRepository;
        }

        public async Task<Page<SearchAggregate>> AggregateSearchAsync(AggregateQueryModel queryModel)
        {
            var (total, data) = await _questionEsRepository.AggregateSearchAsync(queryModel);
            return data.ToPage((int)total);
        }
    }
}
