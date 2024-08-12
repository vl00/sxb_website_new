using System;

namespace Sxb.School.API.Application.Queries.DgAyUserQaPaper
{
    public class DgAyUserQaPaper
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string Title { get; set; }

        public DateTime UnlockedTime { get; set; }

        public int UnlockedType { get; set; }

        public DateTime? LastSubmitTime { get; set; }

        public DateTime? AnalyzedTime { get; set; }


    }
}
