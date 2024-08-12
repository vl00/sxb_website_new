using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Kogel.Dapper.Extension.Attributes;
using Sxb.Domain;

namespace Sxb.SignActivity.Domain.AggregatesModel.SignAggregate
{
    public class SignInParentHistory : Entity<Guid>, IAggregateRoot
    {
        /// <summary> 
        /// 编号 
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary> 
        /// 业务id 
        /// </summary> 
        [Column("bu_no")]
        public string BuNo { get; set; }

        /// <summary> 
        /// 签到用户id 
        /// </summary> 
        [Column("member_id")]
        public Guid MemberId { get; set; }

        /// <summary> 
        /// 扩展字段2，用户openid或者手机号码  
        /// </summary> 
        [Column("param2")]
        public string Param2 { get; set; }

        /// <summary> 
        /// 扩展字段3，钱包中心返回的冻结资金id  
        /// </summary> 
        [Column("param3")]
        public string Param3 { get; set; }

        /// <summary> 
        /// 创建时间 
        /// </summary> 
        [Column("create_time")]
        public DateTime CreateTime { get; set; }

        /// <summary> 
        /// 更新时间 
        /// </summary> 
        [Column("modify_time")]
        public DateTime ModifyTime { get; set; }

        /// <summary> 
        /// 签到次数 
        /// </summary> 
        [Column("parent_invite_count")]
        public int ParentInviteCount { get; set; }

        /// <summary> 
        /// 绑定的父级 
        /// </summary> 
        [Column("parent_id")]
        public Guid ParentId { get; set; }

        /// <summary> 
        /// 累计奖励 
        /// </summary> 
        [Column("parent_total_money")]
        public int ParentTotalMoney { get; set; }

        /// <summary> 
        /// 本次签到奖励金币个数 
        /// </summary> 
        [Column("parent_reward_money")]
        public int ParentRewardMoney { get; set; }

        /// <summary>
        /// 签到日期(单位精确到日)
        /// </summary>
        [Column("sign_in_date")]
        public DateTime SignInDate { get; set; }

        /// <summary> 
        /// 
        /// </summary> 
        [Column("is_valid")]
        public bool IsValid { get; set; }

        /// <summary> 
        /// 
        /// </summary> 
        [Column("blocked")]
        public bool Blocked { get; set; }


        public void UnBlock()
        {
            if (Blocked == false)
            {
                throw new ArgumentException("已解锁");
            }
            Blocked = false;
            ModifyTime = DateTime.Now;
        }

        public void ReBlock()
        {
            Blocked = true;
            ModifyTime = DateTime.Now;
        }
    }
}