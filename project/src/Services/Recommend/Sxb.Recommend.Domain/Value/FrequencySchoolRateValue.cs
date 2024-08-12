using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Value
{
    public class FrequencySchoolRateValue : ValueObject
    {
        public double Rate { get; set; }

        public double Score { get; set; }
        protected override IEnumerable<object> GetAtomicValues()
        {
            throw new NotImplementedException();
        }
    }
}
