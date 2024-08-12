using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 
	/// </summary>
	[Display("CategoryQuestionTop")]
	public partial class CategoryQuestionTop
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 问题
		/// </summary> 
		public Guid QuestionId { get; set; } 

		/// <summary>
		/// 分类
		/// </summary> 
		public long CategoryId { get; set; } 

		/// <summary>
		/// 城市
		/// </summary> 
		public long City { get; set; } 

		/// <summary>
		/// 是否删除
		/// </summary> 
		//dbDefaultValue// select ((1)) 
		public bool IsValid { get; set; } = true; 

	}
}
