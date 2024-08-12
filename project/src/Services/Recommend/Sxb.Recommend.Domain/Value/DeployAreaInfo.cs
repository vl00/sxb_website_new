using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Value
{
    /// <summary>
    /// 投放地区信息
    /// </summary>
    public class DeployAreaInfo : ValueObject
    {
        public int Province { get; set; }

        public int City { get; set; }

        public int Area { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Province;
            yield return City;
            yield return Area;

        }
    }
}
