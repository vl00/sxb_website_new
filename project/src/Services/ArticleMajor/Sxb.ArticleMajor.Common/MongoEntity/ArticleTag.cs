using System;
using System.Collections.Generic;
using Kogel.Dapper.Extension.Attributes;

namespace Sxb.ArticleMajor.Common.MongoEntity
{
	/// <summary> 
	///  
	/// </summary> 
	public partial class ArticleTag
	{
		/// <summary> 
		/// Id 
		/// </summary> 
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