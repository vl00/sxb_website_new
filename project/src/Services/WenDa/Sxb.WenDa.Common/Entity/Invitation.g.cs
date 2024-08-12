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
	[Display("Invitation")]
	public partial class Invitation
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; } 

		/// <summary>
		/// 问题id
		/// </summary> 
		public Guid Qid { get; set; } 

		/// <summary>
		/// 发起人
		/// </summary> 
		public Guid FromUserId { get; set; } 

		/// <summary>
		/// 被邀请人
		/// </summary> 
		public Guid ToUserId { get; set; } 

		/// <summary>
		/// 邀请时间
		/// </summary> 
		public DateTime InviteTime { get; set; } 

		/// <summary>
		/// 是否有效
		/// </summary> 
		//dbDefaultValue// select ((1)) 
		public bool IsValid { get; set; } = true; 

		/// <summary>
		/// 应答时间
		/// </summary> 
		public DateTime? AnswerTime { get; set; } 

	}
}
