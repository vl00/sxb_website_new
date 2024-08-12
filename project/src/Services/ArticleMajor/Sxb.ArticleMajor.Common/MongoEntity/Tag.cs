using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Sxb.ArticleMajor.Common.MongoEntity
{
	/// <summary> 
	///  
	/// </summary> 
	[BsonIgnoreExtraElements]
	public partial class Tag
	{
		/// <summary> 
		/// Id 
		/// </summary> 
		[BsonId]
		public int Id { get; set; }

		/// <summary> 
		/// 名称 
		/// </summary> 
		public string Name { get; set; }

		/// <summary> 
		/// 短编码 
		/// </summary> 
		public string ShortName { get; set; }

		/// <summary> 
		/// 是否删除 
		/// </summary> 
		public bool IsValid { get; set; }
	}
}