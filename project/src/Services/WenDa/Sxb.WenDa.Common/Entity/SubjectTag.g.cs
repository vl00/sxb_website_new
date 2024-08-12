using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 专栏的标签
	/// </summary>
	[Display("SubjectTag")]
	public partial class SubjectTag
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
		public long TagId { get; set; } 

	}
}
