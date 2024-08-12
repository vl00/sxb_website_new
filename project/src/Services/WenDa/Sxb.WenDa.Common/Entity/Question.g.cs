using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 问答广场-问题
	/// </summary>
	[Display("Question")]
	public partial class Question
	{

		/// <summary>
		/// 
		/// </summary> 
		[Identity(false)]
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
		public int? State { get; set; } 

		/// <summary>
		/// 作者
		/// </summary> 
		//dbDefaultValue// select ('00000000-0000-0000-0000-000000000000') 
		public Guid UserId { get; set; } = new Guid("00000000-0000-0000-0000-000000000000"); 

		/// <summary>
		/// 问题标题
		/// </summary> 
		public string Title { get; set; } 

		/// <summary>
		/// 1=匿名 0=非匿名
		/// </summary> 
		public bool IsAnony { get; set; } 

		/// <summary>
		/// 创建时间
		/// </summary> 
		public DateTime CreateTime { get; set; } 

		/// <summary>
		/// 最后一次编辑时间
		/// </summary> 
		public DateTime? LastEditTime { get; set; } 

		/// <summary>
		/// 编辑次数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int EditCount { get; set; } = 0; 

		/// <summary>
		/// 回答数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int ReplyCount { get; set; } = 0; 

		/// <summary>
		/// 收藏数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int CollectCount { get; set; } = 0; 

		/// <summary>
		/// 1=正常 0=已删除
		/// </summary> 
		//dbDefaultValue// select ((1)) 
		public bool IsValid { get; set; } = true; 

		/// <summary>
		/// 点赞数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int LikeCount { get; set; } = 0; 

		/// <summary>
		/// 专题id
		/// </summary> 
		public Guid? SubjectId { get; set; } 

		/// <summary>
		/// 问题补充说明
		/// </summary> 
		public string Content { get; set; } 

		/// <summary>
		/// 图片
		/// </summary> 
		public string Imgs { get; set; } 

		/// <summary>
		/// 图片缩略图
		/// </summary> 
		public string Imgs_s { get; set; } 

		/// <summary>
		/// 问题所属区域-城市
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public long City { get; set; } = 0L; 

		/// <summary>
		/// 问题分类
		/// </summary> 
		public long? CategoryId { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public DateTime? ModifyDateTime { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public Guid? Modifier { get; set; } 

		/// <summary>
		/// 所有回答的点赞总数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int LikeTotalCount { get; set; } = 0; 

		/// <summary>
		/// 站点/一级标签
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public long Platform { get; set; } = 0L; 

		/// <summary>
		/// 是否真实用户 1=是 0=不是
		/// </summary> 
		public bool? IsRealUser { get; set; } 

		/// <summary>
		/// 匿名发布时用的名字
		/// </summary> 
		public string AnonyUserName { get; set; } 

		/// <summary>
		/// 1=主站置顶
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public bool IsTop { get; set; } = false; 

	}
}
