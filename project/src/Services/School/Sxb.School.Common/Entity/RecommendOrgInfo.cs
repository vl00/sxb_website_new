using System;
using System.Collections.Generic;

namespace Sxb.School.Common.Entity
{
    public class RecommendOrgInfo
    {
        public class HttpWrapper
        {
            public IEnumerable<RecommendOrgInfo> Orgs { get; set; }
        }
        public Guid ID { get; set; }
        public string ID_S { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public bool Authentication { get; set; }
        public string Desc { get; set; }
        public string SubDesc { get; set; }
        public int CourceCount { get; set; }
        public int EvaluationCount { get; set; }
    }
}
