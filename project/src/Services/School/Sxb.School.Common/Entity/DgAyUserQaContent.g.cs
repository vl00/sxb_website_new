using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.School.Common.Entity
{ 
	/// <summary>
	/// 问卷内容
	/// </summary>
	[Display("DgAyUserQaContent")]
	public partial class DgAyUserQaContent
	{

		/// <summary>
		/// 
		/// </summary> 		
		public Guid Id { get; set; } 

		/// <summary>
		/// 用户问卷id
		/// </summary> 
		public Guid Qaid { get; set; } 

		/// <summary>
		/// 第几题
		/// </summary> 
		public int? Num { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public bool IsValid { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public int? SubmitCount { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public DateTime? SubmitTime { get; set; } 
		
		/// <summary>
		/// 
		/// </summary> 
		public string Ctn { get; set; } 

	}
}
