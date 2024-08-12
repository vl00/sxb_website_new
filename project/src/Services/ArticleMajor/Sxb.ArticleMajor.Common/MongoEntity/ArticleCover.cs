using System;
using System.Collections.Generic;

namespace Sxb.ArticleMajor.Common.MongoEntity
{
	/// <summary> 
	///  
	/// </summary> 
	public class ArticleCover
	{
		/// <summary> 
		/// 文章id 
		/// </summary> 
		public Guid ArticleId { get; set; }

		/// <summary> 
		/// 图片id 
		/// </summary> 
		public int ImageId { get; set; }

		/// <summary> 
		/// 图片链接 
		/// </summary> 
		public string ImageUrl { get; set; }

		/// <summary> 
		/// 排序 
		/// </summary> 
		public int Sort { get; set; }

		/// <summary> 
		/// 是否删除 
		/// </summary> 
		public bool IsValid { get; set; }


	}
}