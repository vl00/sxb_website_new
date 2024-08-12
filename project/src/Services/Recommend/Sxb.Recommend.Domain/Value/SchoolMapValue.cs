using Sxb.Domain;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Value
{
    public class SchoolMapValue: ValueObject
    {
        public double Score { get; set; }
        public string Remark { get; set; }

        public School SchoolP { get; set; }
        public School SchoolS { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            return new List<object>() { SchoolP.Id, SchoolS.Id, Score };
        }
    }
}
