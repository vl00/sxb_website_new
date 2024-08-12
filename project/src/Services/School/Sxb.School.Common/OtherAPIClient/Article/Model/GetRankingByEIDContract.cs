using System;

namespace Sxb.School.Common.OtherAPIClient.Article.Model
{
    public class GetRankingByEIDResponse
    {
        public Guid RankID { get; set; }
        public string RankName { get; set; }
        public int Ranking { get; set; }
        public string Cover { get; set; }
    }
}
