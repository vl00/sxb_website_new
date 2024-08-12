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
	public class ArticleDynamic
	{
		/// <summary> 
		/// ArticleId 
		/// </summary> 
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string ArticleId { get; set; }

		/// <summary>
		/// 页面浏览数
		/// </summary>
		public int ViewCount { get; set; }
	}
}