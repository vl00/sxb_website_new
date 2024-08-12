using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 回答
	/// </summary>
	[Display("QuestionAnswer")]
	public partial class QuestionAnswer
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
		/// 问题id
		/// </summary> 
		public Guid Qid { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public int? State { get; set; } 

		/// <summary>
		/// 用户id/层主
		/// </summary> 
		public Guid UserId { get; set; } 

		/// <summary>
		/// 1=匿名 0=非匿名
		/// </summary> 
		public bool IsAnony { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public string Content { get; set; } 

		/// <summary>
		/// 创建于
		/// </summary> 
		public DateTime CreateTime { get; set; } 

		/// <summary>
		/// 编辑于
		/// </summary> 
		public DateTime? LastEditTime { get; set; } 

		/// <summary>
		/// 编辑次数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int EditCount { get; set; } = 0; 

		/// <summary>
		/// 点赞数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int LikeCount { get; set; } = 0; 

		/// <summary>
		/// 评论数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int ReplyCount { get; set; } = 0; 

		/// <summary>
		/// 1=正常; 0=已删除
		/// </summary> 
		public bool IsValid { get; set; } 

		/// <summary>
		/// 图
		/// </summary> 
		public string Imgs { get; set; } 

		/// <summary>
		/// 缩略图
		/// </summary> 
		public string Imgs_s { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public DateTime? ModifyDateTime { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public Guid? Modifier { get; set; } 

		/// <summary>
		/// 匿名发布时用的名字
		/// </summary> 
		public string AnonyUserName { get; set; } 

	}
}
