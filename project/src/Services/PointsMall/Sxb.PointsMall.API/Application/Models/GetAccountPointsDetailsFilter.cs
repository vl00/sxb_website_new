using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Models
{
    [Description("获取积分明细过滤")]
    public class GetAccountPointsDetailsFilter
    {
        [Description("用户ID")]
        public Guid  UserId { get; set; }
        [Description("查询方向，0-> 消耗，1->奖励")]
        public bool Direction { get; set; }
        [Description("开始时间")]
        public DateTime STime { get; set; }
        [Description("结束时间")]
        public DateTime ETime { get; set; }
        [Description("偏移量")]
        public int Offset { get; set; } = 0;
        [Description("最大返回量")]
        public int Limit { get; set; } = 20;
    }
}
