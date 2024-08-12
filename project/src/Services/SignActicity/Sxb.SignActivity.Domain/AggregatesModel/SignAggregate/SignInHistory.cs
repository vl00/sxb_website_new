using Sxb.Domain;
using Sxb.SignActivity.Domain.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sxb.SignActivity.Domain.AggregatesModel.SignAggregate
{
	public class SignInHistory : Entity<Guid>, IAggregateRoot
	{
		/// <summary> 
		/// 业务id 
		/// </summary> 
		[Column("bu_no")] 
		public string BuNo {get;set;}

		/// <summary> 
		/// 签到用户id 
		/// </summary> 
		[Column("member_id")] 
		public Guid? MemberId {get;set;}

		/// <summary> 
		/// 签到日期(单位精确到日) 
		/// </summary> 
		[Column("sign_in_date")] 
		public DateTime? SignInDate {get;set;}

		/// <summary> 
		/// 本次签到奖励金币个数 
		/// </summary> 
		[Column("reward_money")] 
		public int RewardMoney {get;set;}

		/// <summary> 
		/// 连续签到天数（A:7天内如果有断签从0开始 B:7天签满从0开始） 
		/// </summary> 
		[Column("continuite_day")] 
		public int? ContinuiteDay {get;set;}

		/// <summary> 
		/// 创建者 
		/// </summary> 
		public Guid? Creator {get;set;}

		/// <summary> 
		/// 创建时间 
		/// </summary> 
		public DateTime? CreateTime {get;set;}

		/// <summary> 
		/// 更新者 
		/// </summary> 
		public Guid? Modifier {get;set;}

		/// <summary> 
		/// 更新时间 
		/// </summary> 
		public DateTime? ModifyDateTime {get;set;}

		/// <summary> 
		/// 删除标志 true/false 删除/未删除 
		/// </summary> 
		public bool? IsValid {get;set;}

		/// <summary> 
		/// </summary> 
		[Column("id")] 
		public Guid Id {get;set;}

		/// <summary> 
		/// 累计奖金 
		/// </summary> 
		[Column("total_money")] 
		public int? TotalMoney {get;set;}

		/// <summary> 
		/// 扩展字段1,订单advanceid 
		/// </summary> 
		[Column("param1")] 
		public string Param1 {get;set;}

		/// <summary> 
		/// 下次签到奖励金币个数 
		/// </summary> 
		[Column("next_reward_money")] 
		public int? NextRewardMoney {get;set;}

		/// <summary> 
		/// 剩余签到次数 
		/// </summary> 
		[Column("left_sign_count")] 
		public int? LeftSignCount {get;set;}

		/// <summary> 
		/// 奖金是否锁定 
		/// </summary> 
		[Column("blocked")] 
		public bool? Blocked {get;set;}

		/// <summary> 
		/// 签到次数 
		/// </summary> 
		[Column("sign_count")] 
		public int SignCount {get;set;}

		/// <summary> 
		/// 扩展字段2，用户openid或者手机号码 
		/// </summary> 
		[Column("param2")] 
		public string Param2 {get;set;}

		/// <summary> 
		/// 扩展字段3，钱包中心返回的冻结资金id 
		/// </summary> 
		[Column("param3")] 
		public string Param3 {get;set;}

		/// <summary> 
		/// 解冻的签到日期
		/// </summary> 
		[Column("unblock_sign_in_date")]
        public DateTime? UnblockSignInDate { get; set; }
		
		public void UnBlock(Guid modifier, DateTime unblockSignInDate)
        {
            if (Blocked == false)
            {
				throw new ArgumentException("已解锁");
            }
			Blocked = false;
			UnblockSignInDate = unblockSignInDate;
			Modifier = modifier;
			ModifyDateTime = DateTime.Now;
		}

		public void ReBlock(Guid modifier)
		{
			Blocked = true;
			UnblockSignInDate = null;
			Modifier = modifier;
			ModifyDateTime = DateTime.Now;
		}
	}
}