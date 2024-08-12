using System;

namespace Sxb.PointsMall.API.Application.IntegrationEvents
{
    public class AddChildIntegrationEvent
    {
        /// <summary>
        /// 孩子Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 添加人id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 孩子名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
