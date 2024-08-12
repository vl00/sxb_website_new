using System;

namespace Sxb.PointsMall.API.Application.IntegrationEvents
{
    public class AddEvaluationIntegrationEvent
    {
        /// <summary>
        /// 种草Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 种草用户
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 种草标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 种草时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
