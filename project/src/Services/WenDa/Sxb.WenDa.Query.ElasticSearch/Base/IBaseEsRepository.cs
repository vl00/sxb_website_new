using Nest;

namespace Sxb.WenDa.Query.ElasticSearch
{
    /// <summary>
    /// es基类
    /// es官网 https://www.elastic.co/
    /// script参考 https://www.elastic.co/guide/en/elasticsearch/painless/6.2/painless-api-reference.html
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseEsRepository<T> where T : BaseEsModel
    {
        /// <summary>
        /// 创建新索引
        /// </summary>
        /// <param name="forceDelete">删除旧索引</param>
        /// <returns></returns>
        CreateIndexResponse CreateIndex(bool forceDelete = false);
        BulkResponse BulkAdd(EsIndexConfig.EsIndex index, List<T> list);
        BulkResponse BulkUpdate(EsIndexConfig.EsIndex index, List<T> list);
        UpdateByQueryResponse UpdateByQuery(EsIndexConfig.EsIndex index, QueryContainerDescriptor<T> query, IScript script);
    }

}