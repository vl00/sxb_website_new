using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 城市与是否开通问答广场
	/// </summary>
	[Display("CityInfo")]
	public partial class CityInfo
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 城市code
		/// </summary> 
		public long City { get; set; } 

		/// <summary>
		/// 城市名
		/// </summary> 
		public string CityName { get; set; } 

		/// <summary>
		/// 1=已开放; 2=未开放
		/// </summary> 
		public bool IsOpen { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public DateTime? CreateTime { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public Guid? Creator { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		//dbDefaultValue// select ((1)) 
		public bool IsValid { get; set; } = true; 

		/// <summary>
		/// 短名
		/// </summary> 
		public string ShortName { get; set; } 

	}
}
