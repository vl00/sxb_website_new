using System;

namespace Sxb.School.Common.OtherAPIClient.Article.Model.Entity
{
    public class RecommendOrg
    {

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