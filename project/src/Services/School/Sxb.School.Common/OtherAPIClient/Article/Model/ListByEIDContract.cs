using System;

namespace Sxb.School.Common.OtherAPIClient.Article.Model
{
    public class ListByEIDResponse
    {
        public Guid ID { get; set; }
        public DateTime Time { get; set; }
        public int Layout { get; set; }
        public string Title { get; set; }
        public int ViewCount { get; set; }
        public int No { get; set; }
    }
}
