using System;
using System.Collections.Generic;
using Kogel.Dapper.Extension.Attributes;

namespace Sxb.SignActivity.Common.Entity
{
	/// <summary> 
	///  
	/// </summary> 
	[Serializable]
	[Display(Rename = "sign_config")]
	public partial class SignConfig
	{
		/// <summary> 
		/// 业务id 
		/// </summary> 
		[Display("bu_no")] 
		public string BuNo {get;set;}

		/// <summary> 
		/// 配置生效起始时间 
		/// </summary> 
		[Display("start_time")] 
		public DateTime StartTime {get;set;}

		/// <summary> 
		/// 配置生效结束时间 
		/// </summary> 
		[Display("end_time")] 
		public DateTime EndTime {get;set;}

		/// <summary> 
		/// 是否可用 
		/// </summary> 
		[Display("isvalid")] 
		public bool Isvalid {get;set;}

		/// <summary> 
		/// 周期内签到最多次数 
		/// </summary> 
		[Display("threshold")] 
		public int Threshold {get;set;}

		/// <summary> 
		/// 奖励信息json字符串 
		/// </summary> 
		[Display("reward_list")] 
		public string RewardList {get;set;}

		/// <summary> 
		/// </summary> 
		[Display("user_limit")] 
		public int UserLimit {get;set;}


	}
}