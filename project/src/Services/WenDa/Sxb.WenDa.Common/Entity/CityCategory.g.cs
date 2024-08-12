using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 城市与分类标签
	/// </summary>
	[Display("CityCategory")]
	public partial class CityCategory
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 城市编码
		/// </summary> 
		public long City { get; set; } 

		/// <summary>
		/// 分类id
		/// </summary> 
		public long CategoryId { get; set; } 

	}
}
