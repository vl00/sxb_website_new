using System;
using System.Collections.Generic;
using Kogel.Dapper.Extension.Attributes;

namespace Sxb.SignActivity.Common.Entity
{
	/// <summary> 
	///  
	/// </summary> 
	[Serializable]
	[Display(Rename = "sign_in_history")]
	public partial class SignInHistory
	{
		/// <summary> 
		/// 业务id 
		/// </summary> 
		[Display("bu_no")] 
		public string BuNo {get;set;}

		/// <summary> 
		/// 签到用户id 
		/// </summary> 
		[Display("member_id")] 
		public Guid MemberId {get;set;}

		/// <summary> 
		/// 签到日期(单位精确到日) 
		/// </summary> 
		[Display("sign_in_date")] 
		public DateTime SignInDate {get;set;}

		/// <summary> 
		/// 本次签到奖励金币个数 
		/// </summary> 
		[Display("reward_money")] 
		public int RewardMoney {get;set;}

		/// <summary> 
		/// 连续签到天数（A:7天内如果有断签从0开始 B:7天签满从0开始） 
		/// </summary> 
		[Display("continuite_day")] 
		public int ContinuiteDay {get;set;}

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
		[Display("id")] 
		public Guid Id {get;set;}

		/// <summary> 
		/// 累计奖金 
		/// </summary> 
		[Display("total_money")] 
		public int TotalMoney {get;set;}

		/// <summary> 
		/// 扩展字段1 
		/// </summary> 
		[Display("param1")] 
		public string Param1 {get;set;}

		/// <summary> 
		/// 下次签到奖励金币个数 
		/// </summary> 
		[Display("next_reward_money")] 
		public int NextRewardMoney {get;set;}

		/// <summary> 
		/// 剩余签到次数 
		/// </summary> 
		[Display("left_sign_count")] 
		public int LeftSignCount {get;set;}

		/// <summary> 
		/// 奖金是否锁定 
		/// </summary> 
		[Display("blocked")] 
		public bool Blocked {get;set;}

		/// <summary> 
		/// 签到次数 
		/// </summary> 
		[Display("sign_count")] 
		public int SignCount {get;set;}

		/// <summary> 
		/// 扩展字段2 
		/// </summary> 
		[Display("param2")] 
		public string Param2 {get;set;}

		/// <summary> 
		/// 扩展字段3 
		/// </summary> 
		[Display("param3")] 
		public string Param3 {get;set;}

		/// <summary> 
		/// 解冻的签到日期 
		/// </summary> 
		[Display("unblock_sign_in_date")] 
		public DateTime? UnblockSignInDate {get;set;}


	}
}