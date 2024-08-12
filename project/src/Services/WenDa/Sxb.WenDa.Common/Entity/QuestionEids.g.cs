using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace Sxb.WenDa.Common.Entity
{ 
	/// <summary>
	/// 问题关联的学部eids
	/// </summary>
	[Display("QuestionEids")]
	public partial class QuestionEids
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
		/// 关联的学部eid
		/// </summary> 
		public Guid Eid { get; set; } 

	}
}
