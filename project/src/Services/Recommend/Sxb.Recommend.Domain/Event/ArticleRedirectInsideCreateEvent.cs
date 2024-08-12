using Sxb.Domain;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Event
{
    public class ArticleRedirectInsideCreateEvent : IDomainEvent
    {
        public ArticleRedirectInside ArticleRedirectInside { get;private set; }
        public ArticleRedirectInsideCreateEvent(ArticleRedirectInside articleRedirectInside)
        {
            ArticleRedirectInside = articleRedirectInside;
        }
    }
}
