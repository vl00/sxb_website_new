using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Value
{
    public class ArticleRedirectFrequencyValue : ValueObject
    {

        public Guid AIdP { get;  set; }

        public Guid AIdS { get;  set; }


        /// <summary>
        /// 打开次数
        /// </summary>
        public int OpenTimes { get;  set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            throw new NotImplementedException();
        }
    }
}
