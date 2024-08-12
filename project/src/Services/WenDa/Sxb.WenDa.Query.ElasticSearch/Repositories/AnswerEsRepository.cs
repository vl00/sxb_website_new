using Microsoft.Extensions.Options;
using Nest;
using ProductManagement.Framework.SearchAccessor;
using Sxb.Framework.Foundation.Maker;
using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;

namespace Sxb.WenDa.Query.ElasticSearch
{
    public class AnswerEsRepository : BaseEsRepository<SearchAnswer>, IAnswerEsRepository
    {
        public AnswerEsRepository(ISearch search, IOptions<EsIndexConfig> options)
            : base(search, options.Value.Indices.Answer)
        {
        }

        public void AddTest()
        {
            var testData = DataMakerHelper.Makes<SearchAnswer>(100).ToList();
            var bulkAddResponse = BulkAdd(_index, testData);
        }

        public override CreateIndexResponse CreateIndex()
        {
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
                .Map<SearchAnswer>(mm => mm
                .Dynamic(false)
                    .Properties(p => p
                        //.Keyword(t => t.Name(n => n.RefTable))
                        .Keyword(t => t.Name(n => n.Id))
                        .Keyword(t => t.Name(n => n.QId))
                        .Keyword(t => t.Name(n => n.UserId))
                        .Text(t => t.Name(n => n.Content).SearchText())
                        .Boolean(t => t.Name(n => n.IsAnony))
                        .Number(t => t.Name(n => n.EditCount))
                        .Number(t => t.Name(n => n.ReplyCount))
                        .FieldAlias(f => f.Name("AnswerReplyCount").Path(n => n.ReplyCount))
                        .Number(t => t.Name(n => n.LikeCount))
                        .Boolean(t => t.Name(n => n.IsValid))
                        .Date(t => t.Name(n => n.CreateTime))
                        .Date(t => t.Name(n => n.LastEditTime))
                        .Date(t => t.Name(n => n.ModifyDateTime))
                    )
                )
            );
            return createIndexResponse;
        }
    }
}