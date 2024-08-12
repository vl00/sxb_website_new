using MongoDB.Bson;
using Sxb.Infrastructure.Core;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.IRepository
{
    public interface IArticleRedirectInsideRepository : IRepository<ArticleRedirectInside, ObjectId>
    {
        /// <summary>
        /// 统计文章 p->s 的跳转频率
        /// </summary>
        /// <param name="aidp"></param>
        /// <param name="aids"></param>
        /// <returns></returns>
        Task<ArticleRedirectFrequencyValue> StaticFrequencyAsync(Guid aidp, Guid aids);
    }
}
