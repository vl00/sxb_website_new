using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Query.ElasticSearch.Models
{
    public class SearchAnswer : BaseEsModel
	{
		/// <summary>
		/// 作者
		/// </summary> 
		public Guid QId { get; set; }

		/// <summary>
		/// 作者
		/// </summary> 
		public Guid UserId { get; set; }

		/// <summary>
		/// 回答内容
		/// </summary> 
		public string Content { get; set; }

		/// <summary>
		/// 1=匿名 0=非匿名
		/// </summary> 
		public bool IsAnony { get; set; }

		/// <summary>
		/// 编辑次数
		/// </summary> 
		public int EditCount { get; set; } = 0;

		/// <summary>
		/// 点赞数
		/// </summary> 
		//dbDefaultValue// select ((0)) 
		public int LikeCount { get; set; } = 0;

		/// <summary>
		/// 回答数
		/// </summary> 
		public int ReplyCount { get; set; } = 0;

		/// <summary>
		/// 问题所属区域-城市
		/// </summary> 
		public long City { get; set; } = 0L;

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
	}
}
