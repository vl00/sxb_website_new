using Sxb.Domain;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Value
{
    public class ArticleMapValue : ValueObject
    {
        public double Score { get; set; }
        public string Remark { get; set; }

        public Article ArticleP { get; set; }
        public Article ArticleS { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            return new List<object>() { ArticleP.Id, ArticleS.Id, Score };
        }
    }
}
