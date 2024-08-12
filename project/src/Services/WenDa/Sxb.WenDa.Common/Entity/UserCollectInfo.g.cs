using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 详细收藏info
	/// </summary>
	[Display("UserCollectInfo")]
	public partial class UserCollectInfo
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 1=问答帖子; 2=专(题)栏
		/// </summary> 
		public byte Type { get; set; } 

		/// <summary>
		/// 源id
		/// </summary> 
		public Guid DataId { get; set; } 

		/// <summary>
		/// 用户id
		/// </summary> 
		public Guid UserId { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public DateTime CreateTime { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		public bool IsValid { get; set; } 

	}
}
