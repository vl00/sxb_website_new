using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 专栏/专题
	/// </summary>
	[Display("QaSubject")]
	public partial class QaSubject
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		//[Write(false)] 
		//db column is AutoIncrement 
		public long No { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public string Img { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public string Img_s { get; set; } 

		/// <summary>
		/// 标题
		/// </summary> 
		public string Title { get; set; } 

		/// <summary>
		/// 描述/副标题
		/// </summary> 
		public string Content { get; set; } 

		/// <summary>
		/// 浏览数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int ViewCount { get; set; } = 0; 

		/// <summary>
		/// 收藏数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int CollectCount { get; set; } = 0; 

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
		public DateTime? ModifyDateTime { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public Guid? Modifier { get; set; } 

	}
}
