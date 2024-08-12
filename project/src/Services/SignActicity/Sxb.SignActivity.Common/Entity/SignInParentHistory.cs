using System;
using System.Collections.Generic;
using Kogel.Dapper.Extension.Attributes;

namespace Sxb.SignActivity.Common.Entity
{
	/// <summary> 
	///  
	/// </summary> 
	[Serializable]
	[Display(Rename = "sign_in_parent_history")]
	public partial class SignInParentHistory
	{
		/// <summary> 
		/// 编号 
		/// </summary> 
		public Guid Id {get;set;}

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
		/// 扩展字段2，用户openid或者手机号码  
		/// </summary> 
		[Display("param2")] 
		public string Param2 {get;set;}

		/// <summary> 
		/// 扩展字段3，钱包中心返回的冻结资金id  
		/// </summary> 
		[Display("param3")] 
		public string Param3 {get;set;}

		/// <summary> 
		/// 创建时间 
		/// </summary> 
		[Display("create_time")] 
		public DateTime CreateTime {get;set;}

		/// <summary> 
		/// 更新时间 
		/// </summary> 
		[Display("modify_time")] 
		public DateTime ModifyTime {get;set;}

		/// <summary> 
		/// 签到次数 
		/// </summary> 
		[Display("parent_invite_count")] 
		public int ParentInviteCount {get;set;}

		/// <summary> 
		/// 绑定的父级 
		/// </summary> 
		[Display("parent_id")] 
		public Guid ParentId {get;set;}

		/// <summary> 
		/// 本次签到奖励金币个数 
		/// </summary> 
		[Display("parent_reward_money")] 
		public int ParentRewardMoney {get;set;}

		/// <summary> 
		/// 是否有效 
		/// </summary> 
		[Display("is_valid")] 
		public bool IsValid {get;set;}

		/// <summary> 
		/// 累计奖励 
		/// </summary> 
		[Display("parent_total_money")] 
		public int ParentTotalMoney {get;set;}

		/// <summary> 
		/// 签到时间 
		/// </summary> 
		[Display("sign_in_date")] 
		public DateTime SignInDate {get;set;}

		/// <summary> 
		/// 是否冻结 
		/// </summary> 
		[Display("blocked")] 
		public bool Blocked {get;set;}


	}
}