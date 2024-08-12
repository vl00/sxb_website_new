using System;
using System.Collections.Generic;
using Kogel.Dapper.Extension.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using Sxb.ArticleMajor.Common.Enum;

namespace Sxb.ArticleMajor.Common.MongoEntity
{
	/// <summary> 
	///  
	/// </summary> 
	[BsonIgnoreExtraElements]
	public class Category
	{
		/// <summary> 
		/// 节点Id 
		/// </summary> 
		[BsonId]
		public int Id { get; set; }

		/// <summary> 
		/// 节点名称 
		/// </summary> 
		public string Name { get; set; }

		/// <summary> 
		/// 父节点 
		/// </summary> 
		public int ParentId { get; set; }

		/// <summary> 
		/// 自定义短链接,否则为节点名称全拼 
		/// </summary> 
		public string ShortName { get; set; }

		/// <summary> 
		/// 是否是叶子节点
		/// </summary> 
		public bool IsLeaf { get; set; }

		/// <summary>
		/// 层级
		/// </summary>
		public int Depth { get; set; }

		/// <summary>
		/// 是否删除
		/// </summary>
		public bool IsValid { get; set; }

		/// <summary>
		/// 归属平台
		/// </summary>
		public ArticlePlatform Platform { get; set; }
	}
}