using Newtonsoft.Json;
using System;

namespace Sxb.ArticleMajor.API.Application.IntegrationEvents
{
    public class ViewArticleIntegrationEvent
    {
        public ViewArticleIntegrationEvent(string id, DateTime time, Guid? userId)
        {
            Id = id;
            Time = time;
            UserId = userId;
            NonceStr = Guid.NewGuid().ToString("N");
        }

        public string Id { get; set; }

        public DateTime Time { get; set; }

        public Guid? UserId { get; set; }

        public string NonceStr { get; set; }
    }
}
