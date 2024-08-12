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
	[Display("SubjectQuestionTop")]
	public partial class SubjectQuestionTop
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 置顶问题
		/// </summary> 
		public Guid QuestionId { get; set; } 

		/// <summary>
		/// 专栏id
		/// </summary> 
		public Guid SubjectId { get; set; } 

		/// <summary>
		/// 城市
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public long City { get; set; } = 0L; 

		/// <summary>
		/// 是否删除
		/// </summary> 
		//dbDefaultValue// select ((1)) 
		public bool IsValid { get; set; } = true; 

	}
}
