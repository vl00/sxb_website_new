using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Query.ElasticSearch.Models
{
    public class SearchQuestion : BaseEsModel
	{
		/// <summary>
		/// 
		/// </summary> 
		public long No { get; set; }

		/// <summary>
		/// 作者
		/// </summary> 
		public Guid UserId { get; set; }

		/// <summary>
		/// 专题id
		/// </summary> 
		public Guid? SubjectId { get; set; }

		/// <summary>
		/// 问题标题
		/// </summary> 
		public string Title { get; set; }

		/// <summary>
		/// 问题补充说明
		/// </summary> 
		public string Content { get; set; }

		/// <summary>
		/// 回答数
		/// </summary> 
		public int ReplyCount { get; set; } = 0;

		/// <summary>
		/// 收藏数
		/// </summary> 
		public int CollectCount { get; set; } = 0;

		/// <summary>
		/// 点赞数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int LikeCount { get; set; } = 0;

		/// <summary>
		/// 问题所属区域-城市
		/// </summary> 
		public long City { get; set; } = 0L;

		/// <summary>
		/// 问题分类
		/// </summary> 
		public long CategoryId { get; set; }

		/// <summary>
		/// 问题标签s
		/// </summary> 
		public List<long> TagIds { get; set; }

		/// <summary>
		/// 1=正常 0=已删除
		/// </summary> 
		public bool IsValid { get; set; } = true;

		/// <summary>
		/// 创建时间
		/// </summary> 
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// 最后一次编辑时间
		/// </summary> 
		public DateTime LastEditTime { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary> 
		public DateTime ModifyDateTime { get; set; }

		/// <summary>
		/// 关联的学校
		/// </summary>
		public List<string> SchoolNames { get; set; }

		/// <summary>
		/// 默认排序的第一条回答(默认点赞数排序)
		/// </summary>
		public Answer BestMatchAnswer { get; set; }

	}
	public class Answer
	{
		public Guid Id { get; set; }

		public string Content { get; set; }
	}
}
