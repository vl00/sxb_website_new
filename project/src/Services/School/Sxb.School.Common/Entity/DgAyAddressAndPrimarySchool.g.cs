using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.School.Common.Entity
{ 
	/// <summary>
	/// 地址对应对口小学
	/// </summary>
	[Display("DgAyAddressAndPrimarySchool")]
	public partial class DgAyAddressAndPrimarySchool
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 市
		/// </summary> 
		public int City { get; set; } 

		/// <summary>
		/// 区
		/// </summary> 
		public int Area { get; set; } 

		/// <summary>
		/// 房产地址
		/// </summary> 
		public string Address { get; set; } 

		/// <summary>
		/// 小学eid
		/// </summary> 
		public Guid? Eid { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public int? Sort { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public DateTime? CreateTime { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public bool IsValid { get; set; } 

	}
}
