using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ProductManagement.Framework.MongoDb;
using Sxb.ArticleMajor.Common.MongoEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb
{
    public class ArticleDynamicRepository : RepositoryBase<ArticleDynamic>, IArticleDynamicRepository
    {

        public ArticleDynamicRepository(IMongoService mongo) : base(mongo)
        {
        }

        public async Task IncreaseViewCountAsync(string articleId)
        {
             await _collection.FindOneAndUpdateAsync<ArticleDynamic>(
                 item => item.ArticleId == articleId, 
                 "{ $inc : { 'ViewCount' : 1} }", 
                 new FindOneAndUpdateOptions<ArticleDynamic, ArticleDynamic>() { IsUpsert = true }) ;
        }
    }
}
