using Elasticsearch.Net;
using Nest;
using Sxb.Framework.SearchAccessor;
using Sxb.WeWorkFinance.API.Application.ES.SearchModels;
using Sxb.WeWorkFinance.API.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.WeWorkFinance.API.Application.ES
{
    public class ChatDataES: IChatDataES
    {
        private readonly ElasticClient _client;
        public ChatDataES(ISearch clientProvider)
        {
            _client = clientProvider.GetClient();
        }


        
        public void ImportChatData(List<MessageModel> messages)
        {
            string indexName = "searchindex_wxworkchatdata_"+DateTime.Now.ToString("yyyy.MM.dd");
            var bulkRequest = messages.SelectMany(m =>
            new[]
            {
                _client.SourceSerializer.SerializeToString(new
                    {
                        index = new
                        {
                            _index = indexName,
                            _type = "_doc",
                            _id = m.Id
                        }
                    }, SerializationFormatting.None),
                m.MessageJson
            });

            var response = _client.LowLevel.Bulk<BulkResponse>(string.Join("\n", bulkRequest) + "\n");
        }
        
    }
}
