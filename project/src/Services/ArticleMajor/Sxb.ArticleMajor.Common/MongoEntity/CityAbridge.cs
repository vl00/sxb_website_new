using System;
using System.Collections.Generic;
using Kogel.Dapper.Extension.Attributes;

namespace Sxb.ArticleMajor.Common.MongoEntity
{
	/// <summary> 
	///  
	/// </summary> 
	public partial class CityAbridge
	{
		/// <summary> 
		/// </summary> 
		public int Id { get; set; }

		/// <summary> 
		/// 首字母简拼 
		/// </summary> 
		public string FirstPinyin { get; set; }


	}
}