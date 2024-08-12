using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sxb.ArticleMajor.Common.Enum;

namespace Sxb.ArticleMajor.Common.MongoEntity
{
	/// <summary> 
	///  
	/// </summary> 
	[BsonIgnoreExtraElements]
	public class Article
	{
		/// <summary> 
		/// Id 
		/// </summary> 
		[BsonId()]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		/// <summary> 
		/// 雪花编码 
		/// </summary> 
		public string Code { get; set; }

		/// <summary> 
		/// 文章标题 
		/// </summary> 
		public string Title { get; set; }

		/// <summary> 
		/// 文章作者 
		/// </summary> 
		public string Author { get; set; }

		/// <summary> 
		/// 是否删除 
		/// </summary> 
		public bool IsValid { get; set; }

		/// <summary> 
		/// 文章发布时间 
		/// </summary> 
		public DateTime PublishTime { get; set; }

		/// <summary> 
		/// 创建时间 
		/// </summary> 
		public DateTime CreateTime { get; set; }

		/// <summary> 
		/// 创建人 
		/// </summary> 
		public string CreatorName { get; set; }

		/// <summary> 
		/// 摘要 
		/// </summary> 
		[BsonIgnoreIfDefault]
		public string Abstract { get; set; }

		/// <summary> 
		/// 来源  网络来源/未知来源 
		/// </summary> 
		[BsonIgnoreIfDefault]
		public string FromWhere { get; set; }

		/// <summary> 
		/// 来源url
		/// </summary> 
		[BsonIgnoreIfDefault]
		public string FromUrl { get; set; }

		/// <summary> 
		/// 投放平台 
		/// </summary>
		public ArticlePlatform Platform { get; set; }

		public int CategoryId { get; set; }

		public List<int> TagIds { get; set; }

		public int CityId { get; set; }

		public List<string> Covers { get; set; }

		/// <summary>
		/// 页面总数
		/// </summary>
		public int PageCount { get; set; }
	}
}