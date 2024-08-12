using Microsoft.Extensions.Options;
using Nest;
using ProductManagement.Framework.SearchAccessor;
using Sxb.Framework.Foundation.Maker;
using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;

namespace Sxb.WenDa.Query.ElasticSearch
{
    public class SubjectEsRepository : BaseEsRepository<SearchSubject>, ISubjectEsRepository
    {
        public SubjectEsRepository(ISearch search, IOptions<EsIndexConfig> options) 
            : base(search, options.Value.Indices.Subject)
        {
        }

        public void AddTest()
        {
            var testData = DataMakerHelper.Makes<SearchSubject>(20).ToList();
            var bulkAddResponse = BulkAdd(_index, testData);
        }

        public override CreateIndexResponse CreateIndex()
        {
            //var deleteIndexResponse = _client.Indices.Delete(_index.Name);
            var createIndexResponse = _client.Indices.Create(_index.Name, c => c
                .Aliases(ad => ad.Alias(_index.Alias))
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Similarity(si => si
                        .BM25("un_norms_similarity", bm25 => bm25.B(0))
                    )
                    .Analysis(AnalyzeBuilder.GetAnalysis())
                )
                .Map<SearchSubject>(mm => mm
                .Dynamic(false)
                    .Properties(p => p
                        .Keyword(t => t.Name(n => n.Id))
                        .Keyword(t => t.Name(n => n.No))
                        .Text(t => t.Name(n => n.Title).SearchText())
                        .Text(t => t.Name(n => n.Content).SearchText())
                        .Number(t => t.Name(n => n.ViewCount))
                        .Number(t => t.Name(n => n.CollectCount))
                        .Boolean(t => t.Name(n => n.IsValid))
                        .Date(t => t.Name(n => n.CreateTime))
                        .Date(t => t.Name(n => n.ModifyDateTime))
                    )
                )
            );
            return createIndexResponse;
        }
    }
}