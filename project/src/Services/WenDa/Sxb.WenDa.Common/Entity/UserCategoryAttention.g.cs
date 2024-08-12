using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 用户关注的类别(领域)
	/// </summary>
	[Display("UserCategoryAttention")]
	public partial class UserCategoryAttention
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 用户id
		/// </summary> 
		public Guid UserId { get; set; } 

		/// <summary>
		/// 分类id
		/// </summary> 
		public long CategoryId { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public DateTime CreateTime { get; set; } 

		/// <summary>
		/// 1=正常 0=删除
		/// </summary> 
		//dbDefaultValue// select ((1)) 
		public bool IsValid { get; set; } = true; 

	}
}
