using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Kogel.Dapper.Extension.Attributes;
using Sxb.Domain;

namespace Sxb.SignActivity.Domain.AggregatesModel.SignAggregate
{
	public class SignConfig : Entity<Guid>, IAggregateRoot
	{
		/// <summary> 
		/// 业务id 
		/// </summary> 
		[Column("bu_no")] 
		public string BuNo {get;set;}

		/// <summary> 
		/// 配置生效起始时间 
		/// </summary> 
		[Column("start_time")] 
		public DateTime StartTime {get;set;}

		/// <summary> 
		/// 配置生效结束时间 
		/// </summary> 
		[Column("end_time")] 
		public DateTime EndTime {get;set;}

		/// <summary> 
		/// 是否可用 
		/// </summary> 
		[Column("isvalid")] 
		public bool? Isvalid {get;set;}

		/// <summary> 
		/// 周期内签到最多次数 
		/// </summary> 
		[Column("threshold")] 
		public int? Threshold {get;set;}

		/// <summary> 
		/// 奖励信息json字符串 
		/// </summary> 
		[Column("reward_list")] 
		public string RewardList {get;set;}


		/// <summary> 
		/// 人数限制
		/// </summary> 
		[Column("user_limit")]
		public int UserLimit { get; set; }
	}
}