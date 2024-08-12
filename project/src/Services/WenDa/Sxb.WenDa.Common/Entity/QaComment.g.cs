using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 回答的评论
	/// </summary>
	[Display("QaComment")]
	public partial class QaComment
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 回答id
		/// </summary> 
		public Guid Aid { get; set; } 

		/// <summary>
		/// 用户id/层主
		/// </summary> 
		public Guid UserId { get; set; } 

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
		/// 下一级评论数/回复数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int ReplyCount { get; set; } = 0; 

		/// <summary>
		/// 此评论回复哪条评论
		/// </summary> 
		public Guid? FromId { get; set; } 

		/// <summary>
		/// 此评论回复谁
		/// </summary> 
		public Guid? FromUserId { get; set; } 

		/// <summary>
		/// 1=正常; 0=已删除
		/// </summary> 
		public bool IsValid { get; set; } 

		/// <summary>
		/// 1=匿名 0=非匿名
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public bool IsAnony { get; set; } = false; 

		/// <summary>
		/// 
		/// </summary> 
		public DateTime? ModifyDateTime { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public Guid? Modifier { get; set; } 

		/// <summary>
		/// (一级)主评论id
		/// </summary> 
		public Guid MainCommentId { get; set; } 

		/// <summary>
		/// (一级)主评论的子级评论数总和
		/// 子级评论时此字段为null
		/// </summary> 
		public int? ReplyTotalCount { get; set; } 

		/// <summary>
		/// 匿名发布时用的名字
		/// </summary> 
		public string AnonyUserName { get; set; } 

	}
}
