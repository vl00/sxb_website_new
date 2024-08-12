using Microsoft.Extensions.Options;
using Nest;
using ProductManagement.Framework.SearchAccessor;
using Sxb.Framework.Foundation.Maker;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Query.ElasticSearch.Base;
using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;
using static Sxb.WenDa.Query.ElasticSearch.EsIndexConfig;

namespace Sxb.WenDa.Query.ElasticSearch
{
    public class QuestionEsRepository : BaseEsRepository<SearchQuestion>, IQuestionEsRepository
    {
        public EsIndices _indices { get; }
        public QuestionEsRepository(ISearch search, IOptions<EsIndexConfig> options)
            : base(search, options.Value.Indices.Question)
        {
            _indices = options.Value.Indices;
        }


        /// <summary>
        /// 需将命中词标注出来。（颜色标注）
        /// 
        /// 专栏展示：
        /// 1、优先显示标题命中词多的专栏。
        /// 2、如果标题命中词，则优先显示专栏简介命中词多的专题。
        /// 
        /// 问答展示：
        /// 1、优先显示问答标题命中词多的内容。
        /// 2、若问答标题命中词数一样或标题无命中，则优先显示第一条回答命中词多的内容。
        /// </summary>
        /// <param name="queryModel"></param>
        public async Task<(long total, List<SearchAggregate> data)> AggregateSearchAsync(AggregateQueryModel queryModel)
        {
            Indices indices = queryModel.Type switch
            {
                RefTable.Question => _indices.Question.SearchIndex,
                RefTable.Subject => _indices.Subject.SearchIndex,
                _ => Indices.Index(_indices.Question.SearchIndex, _indices.Subject.SearchIndex)
            };

            int from = (queryModel.PageIndex - 1) * queryModel.PageSize;
            int size = queryModel.PageSize;
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchQuestion>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.IsValid).Value(true))
            };
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchQuestion>, QueryContainer>>
            {

            };

            //有回复的优先
            shouldQuerys.Add(
                s => new NumericRangeQuery()
                {
                    Field = Infer.Field<SearchQuestion>(s => s.ReplyCount),
                    GreaterThan = 0,
                    Boost = 5
                }
            );

            //搜索关键词
            if (!string.IsNullOrWhiteSpace(queryModel.Keyword))
            {
                var query = new MultiMatchQuery()
                {
                    Fields = Infer.Fields(
                            "title.cntext^5", "title.pinyin^1.1",
                            "content.cntext^5", "content.pinyin^1.1",
                            "schoolNames.cntext^5", "schoolNames.pinyin^1.1",
                            "bestMatchAnswer.content.cntext^2", "bestMatchAnswer.content.pinyin"
                        ),
                    Query = queryModel.Keyword
                };

                mustQuerys.Add(q => query);
            }

            //搜索城市
            if (queryModel.City > 0)
            {
                shouldQuerys.Add(m => m.Term(t => t.Field(tf => tf.City).Value(queryModel.City).Boost(10)));
            }

            var search = new SearchDescriptor<SearchQuestion>()
                .Index(indices)
                .IndicesBoost(ib =>
                {
                    ib.Add(_indices.Subject.SearchIndex, 10000);//所有专栏第一显示
                    ib.Add(_indices.Question.SearchIndex, 5);
                    return ib;
                })
                .Query(q => q.Bool(b =>
                    b.Must(mustQuerys).Should(shouldQuerys)
                ))
                //.Highlight(h => h.Fields(
                //        f => f.Field("title.cntext"),
                //        f => f.Field("title.pinyin"),
                //        f => f.Field("content.cntext"),
                //        f => f.Field("content.pinyin"),
                //        f => f.Field("bestMatchAnswer.content.cntext"),
                //        f => f.Field("bestMatchAnswer.content.pinyin")
                //    ).FragmentSize(9999)
                //)
                .From(from)
                .Size(size);

            var result = await _client.SearchAsync<SearchAggregate>(search);
            var total = result.Total;
            var data = result.Hits.Select(s =>
            {
                s.Source.RefTable = _indices.Subject.Contains(s.Index) ?
                            RefTable.Subject : RefTable.Question;

                return s.Source;
            }).ToList();

            var highs = result.Hits.Select(s => s.Highlight).ToList();
            //HighlightHelper.SetHighlights(ref data, highs, "Title", "Title");
            //HighlightHelper.SetHighlights(ref data, highs, "Content", "Content");

            return (total, data);
        }

        public void AddTest()
        {
            var testData = DataMakerHelper.Makes<SearchQuestion>(100).ToList();
            foreach (var item in testData)
            {
                item.BestMatchAnswer = DataMakerHelper.Make<Answer>();
            }
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
                .Map<SearchQuestion>(mm => mm
                .Dynamic(false)
                    .Properties(p => p
                        .Keyword(t => t.Name(n => n.Id))
                        .Keyword(t => t.Name(n => n.No))
                        .Keyword(t => t.Name(n => n.SubjectId))
                        .Keyword(t => t.Name(n => n.UserId))
                        .Text(t => t.Name(n => n.Title).SearchText())
                        .Text(t => t.Name(n => n.Content).SearchText())
                        .Text(t => t.Name(n => n.SchoolNames).SearchText())
                        //.Boolean(t => t.Name(n => n.IsAnony))
                        //.Number(t => t.Name(n => n.EditCount))
                        .Number(t => t.Name(n => n.ReplyCount))
                        .Number(t => t.Name(n => n.CollectCount))
                        .Number(t => t.Name(n => n.LikeCount))
                        .Keyword(t => t.Name(n => n.City))
                        .Keyword(t => t.Name(n => n.CategoryId))
                        .Keyword(t => t.Name(n => n.TagIds))
                        .Boolean(t => t.Name(n => n.IsValid))
                        .Date(t => t.Name(n => n.CreateTime))
                        .Date(t => t.Name(n => n.LastEditTime))
                        .Date(t => t.Name(n => n.ModifyDateTime))
                        .Object<Answer>(t => t
                             .Name(n => n.BestMatchAnswer)
                             .Properties(ap => ap
                                .Keyword(t => t.Name(n => n.Id))
                                .Text(t => t.Name(n => n.Content).SearchText())
                        ))
                    )
                )
            );
            return createIndexResponse;
        }
    }
}