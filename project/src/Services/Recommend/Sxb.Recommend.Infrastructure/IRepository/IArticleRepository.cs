using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.IRepository
{
    public interface IArticleRepository
    {
        Task<IEnumerable<Article>> GetArticles(DateTime afterTime, int offset = 0,int limit=100);
        Task<(Guid id1, Guid id2)> GetIdByNo(long no1, long no2);
    }
}
