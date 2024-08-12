using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 问题分类
	/// </summary>
	[Display("Category")]
	public partial class Category
	{

		/// <summary>
		/// 
		/// </summary> 
		[Identity(false)] 
		public long Id { get; set; } 

		/// <summary>
		/// 分类名
		/// </summary> 
		public string Name { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public long Parentid { get; set; } = 0L; 

		/// <summary>
		/// 表示第几级
		/// </summary> 
		//dbDefaultValue// select ((1)) 
		public int Depth { get; set; } = 1; 

		/// <summary>
		/// 1=分类 2=标签
		/// </summary> 
		public byte Type { get; set; } 

		/// <summary>
		/// 用于前端发问题选学校
		/// 1=可关联学校；0=不需要关联学校
		/// </summary> 
		public bool CanFindSchool { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public DateTime? CreateTime { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public Guid? Creator { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		//dbDefaultValue// select ((1)) 
		public bool IsValid { get; set; } = true; 

		/// <summary>
		/// 
		/// </summary> 
		public string Path { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public long? Sort { get; set; } 

	}
}
