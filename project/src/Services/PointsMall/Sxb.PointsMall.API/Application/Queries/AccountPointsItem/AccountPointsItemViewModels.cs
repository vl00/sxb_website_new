using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Queries.AccountPointsItem
{
    public class AccountPointsItem
    {
        public long Points { get; set; }

        /// <summary>
        /// 积分来源类型
        /// </summary>
        [Description("积分来源类型")]
        public AccountPointsOriginType OriginType { get; set; }

        /// <summary>
        /// 积分来源ID
        /// </summary>
        [Description("积分来源ID")]
        public string OriginId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [Description("日期")]
        public DateTime ModifyTime { get; set; }
    }

    public class AccountPointsDetails
    {
        /// <summary>
        /// 奖励总计
        /// </summary>
        [Description("奖励总计")]
        public long AwardTotal { get; set; }
        /// <summary>
        /// 消耗总计
        /// </summary>
        [Description("消耗总计")]
        public long ConsumeTotal { get; set; }

        /// <summary>
        ///总计
        /// </summary>
        [Description("总计")]
        public long Total => AwardTotal + ConsumeTotal;

        [Description("明细项")]
        public List<AccountPointsItem> Items { get; set; }
    }
}
