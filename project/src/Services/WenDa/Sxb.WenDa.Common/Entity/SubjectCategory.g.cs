using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 专栏的分类
	/// </summary>
	[Display("SubjectCategory")]
	public partial class SubjectCategory
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public Guid SubjectId { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public long CategoryId { get; set; } 

	}
}
