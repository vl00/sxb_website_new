using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;

namespace Sxb.WenDa.Query.ElasticSearch
{
    public interface ISubjectEsRepository : IBaseEsRepository<SearchSubject>
    {
        void AddTest();
    }
}