using Sxb.Domain;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Value
{
    public class SchoolScore : ValueObject
    {
        public School School { get; set; }

        public double Score { get; set; }

        public string Remark { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            throw new NotImplementedException();
        }
    }
}
