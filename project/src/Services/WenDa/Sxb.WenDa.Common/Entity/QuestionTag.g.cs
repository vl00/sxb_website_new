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
	[Display("QuestionTag")]
	public partial class QuestionTag
	{

		/// <summary>
		/// 
		/// </summary> 
		public Guid Qid { get; set; } 

		/// <summary>
		/// 
		/// </summary> 
		[Identity(false)] 
		public long TagId { get; set; } 

	}
}
