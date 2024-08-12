using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Common.MongoEntity
{
	[BsonIgnoreExtraElements]
	public class ArticleContent
	{
		/// <summary>
		/// 内部_Id
		/// </summary>
		[BsonId]
		public ObjectId Id { get; set; }

		/// <summary> 
		/// </summary> 
		//[BsonGuidRepresentation(GuidRepresentation.Standard)]
		[BsonRepresentation(BsonType.ObjectId)]
		public string ArticleId { get; set; }

		/// <summary> 
		/// </summary> 
		public string Content { get; set; }

		/// <summary> 
		/// </summary> 
		public int Sort { get; set; }
	}
}
