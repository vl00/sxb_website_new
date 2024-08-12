using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.School.Common.Entity
{ 
	/// <summary>
	/// 分析结果内容
	/// </summary>
	[Display("DgAyUserQaResultContent")]
	public partial class DgAyUserQaResultContent
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public Guid Qaid { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public string Ctn { get; set; } 

		/// <summary>
		/// 对口小学eid
		/// </summary> 
		public Guid? CpSchoolEid { get; set; } 

		/// <summary>
		/// 电脑派位-初中eids
		/// </summary> 
		public string CpPcAssignSchoolEids { get; set; } 

		/// <summary>
		/// 对口直升-初中eids
		/// </summary> 
		public string CpHeliSchoolEids { get; set; } 

		/// <summary>
		/// 统筹入学-小学eids
		/// </summary> 
		public string OvSchoolEids { get; set; } 

		/// <summary>
		/// 积分入学-积分
		/// </summary> 
		public double? JfPoints { get; set; } 

		/// <summary>
		/// 积分入学-学部eids
		/// </summary> 
		public string JfSchoolEids { get; set; } 

		/// <summary>
		/// 找民办-民办小学eids
		/// </summary> 
		public string MbSchoolEids { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public bool IsValid { get; set; } 

		/// <summary>
		/// 所有的学部eids
		/// </summary> 
		public string Eids { get; set; } 

	}
}
