using Nest;
using ProductManagement.Framework.SearchAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Query.ElasticSearch
{

    public abstract class BaseEsRepository<T> : IBaseEsRepository<T> where T : BaseEsModel
    {
        public ElasticClient _client { get; }
        public EsIndexConfig.EsIndex _index { get; }

        public BaseEsRepository(ISearch search, EsIndexConfig.EsIndex index)
        {
            _client = search.GetClient();
            _index = index;
        }

        public BulkResponse BulkAdd(EsIndexConfig.EsIndex index, List<T> list)
        {
            var indexResponse = _client.Bulk(s => s.IndexMany(list,
                                    (bulkDescriptor, record) => bulkDescriptor.Index(index.SearchIndex).Document(record).Id(record.Id)
                                ));
            return indexResponse;
        }

        public BulkResponse BulkUpdate(EsIndexConfig.EsIndex index, List<T> list)
        {
            var bulkUpdate = new BulkRequest() { Operations = new List<IBulkOperation>() };
            foreach (var item in list)
            {
                var operation = new BulkUpdateOperation<T, object>(item.Id)
                {
                    Index = index.SearchIndex,
                    Doc = item
                };
                bulkUpdate.Operations.Add(operation);
            }
            var indexResponse = _client.Bulk(bulkUpdate);
            return indexResponse;
        }

        public UpdateByQueryResponse UpdateByQuery(EsIndexConfig.EsIndex index, QueryContainerDescriptor<T> query, IScript script)
        {
            var update = new UpdateByQueryRequest(index.SearchIndex);
            update.Query = query;
            //设为空
            //update.SourceIncludes = new Field[] { "WxName" };
            //设置指定值
            //update.Script = new ScriptDescriptor().Source("ctx._source['WxName'] = 'test' ");
            update.Script = script;

            var indexResponse = _client.UpdateByQuery(update);
            return indexResponse;
        }

        public CreateIndexResponse CreateIndex(bool forceDelete = false)
        {
            if (forceDelete)
            {
                var deleteIndexResponse = _client.Indices.Delete(_index.Name);
            }
            return CreateIndex();
        }

        public abstract CreateIndexResponse CreateIndex();
    }
}
