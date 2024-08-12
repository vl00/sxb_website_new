using Sxb.WenDa.Common.Enums;

namespace Sxb.WenDa.Query.ElasticSearch.Models
{
    public class SearchAggregate
    {
        public RefTable RefTable { get; set; }

        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public Guid? AnswerId { get; set; }
    }
}
