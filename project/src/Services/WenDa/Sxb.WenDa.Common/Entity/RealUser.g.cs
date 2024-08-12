using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 
	/// </summary>
	[Display("RealUser")]
	public partial class RealUser
	{

		/// <summary>
		/// 真实账号的用户id
		/// </summary> 
		public Guid UserId { get; set; } 

		/// <summary>
		/// 1=真实用户 0=虚拟用户
		/// </summary> 
		public bool? IsRealUser { get; set; } 

		/// <summary>
		/// 是否已加企业微信客服
		/// </summary> 
		public bool? HasJoinWxEnt { get; set; } 

	}
}
