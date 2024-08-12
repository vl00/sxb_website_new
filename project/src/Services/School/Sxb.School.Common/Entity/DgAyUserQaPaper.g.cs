using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.School.Common.Entity
{ 
	/// <summary>
	/// 用户答题问卷
	/// </summary>
	[Display("DgAyUserQaPaper")]
	public partial class DgAyUserQaPaper
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public Guid UserId { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public DateTime? CreateTime { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public string Title { get; set; } 

		/// <summary>
		/// 分析类型
		/// 1=对口入学 2=统筹入学 3=积分入学 4=查找心仪民办小学
		/// </summary> 
		public byte Atype { get; set; } 

		/// <summary>
		/// 1=做题阶段，2=已分析出结果, 3=已解锁
		/// </summary> 
		public byte Status { get; set; } 

		/// <summary>
		/// 提交次数
		/// </summary> 
		public int? SubmitCount { get; set; } 

		/// <summary>
		/// 最后一次提交时间
		/// </summary> 
		public DateTime? LastSubmitTime { get; set; } 

		/// <summary>
		/// 分析出结果时间
		/// </summary> 
		public DateTime? AnalyzedTime { get; set; } 

		/// <summary>
		/// 1=免费解锁, 2=解锁x元, 3=解锁1元, 0=无结果变解锁
		/// </summary> 
		public byte? UnlockedType { get; set; } 

		/// <summary>
		/// 解锁时间
		/// </summary> 
		public DateTime? UnlockedTime { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public bool IsValid { get; set; } 

		/// <summary>
		/// 终端类型 1->h5 2->pc 3->小程序
		/// </summary> 
		public byte? Termtyp { get; set; } 

	}
}
