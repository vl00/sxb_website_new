using MongoDB.Bson;
using Sxb.Infrastructure.Core;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.IRepository
{
    public interface IArticleRedirectFrequencyRepository : IRepository<ArticleRedirectFrequency, ObjectId>
    {


        /// <summary>
        /// 获取某篇文章对应其所有文章的打开频率
        /// </summary>
        /// <param name="aidp"></param>
        /// <returns></returns>
        Task<IEnumerable<ArticleRedirectFrequency>> QueryFrequenciesAsync(Guid aidp);


        /// <summary>
        /// 更新或者新增频率记录
        /// </summary>
        /// <param name="redirectFrequency"></param>
        /// <returns></returns>
        Task UpsertFrequency(ArticleRedirectFrequency redirectFrequency);


        /// <summary>
        /// 从仓储获取一个 ArticleRedirectFrequency实体，如果没有则创建该实体。需要注意，创建该实体不意味者该实体已持久化。
        /// 请不要主观认为该实体已持久化到仓储，毕竟它的目的是为了拿到一个 ArticleRedirectFrequency实体。
        /// </summary>
        /// <param name="aidp"></param>
        /// <param name="aids"></param>
        /// <returns></returns>
        Task<ArticleRedirectFrequency> GetOrCreate(Guid aidp,Guid aids);



    }
}
