using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.IRepository
{
    public interface IArticleFileRepository
    {

        bool Exists();
        void Init(IEnumerable<Article> schools);

        IEnumerable<Article> Query(Func<Article, bool> where);
        void Append(Article school);

        void Update(Article school);

        void SaveChange();
    }
}
