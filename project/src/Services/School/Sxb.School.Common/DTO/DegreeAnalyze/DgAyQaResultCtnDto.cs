using Sxb.Framework.Foundation;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sxb.School.Common.DTO
{
	/// <summary>
	/// DgAyUserQaResultContent Ctn
	/// </summary>
	public class DgAyQaResultCtnDto
	{
		/// <summary>
		/// 对口小学eid
		/// </summary> 
		public Guid? CpSchoolEid { get; set; }

		/// <summary>
		/// 对口小学-政策文章
		/// </summary> 
		public DgAySchPcyFileDto CpSchPcyFile { get; set; }

		/// <summary>
		/// 电脑派位-初中eids
		/// </summary> 
		public Guid[] CpPcAssignSchoolEids { get; set; }

		/// <summary>
		/// 电脑派位-政策文章
		/// </summary> 
		public DgAySchPcyFileDto CpPcAssignSchPcyFile { get; set; }

		/// <summary>
		/// 对口直升-初中eids
		/// </summary> 
		public Guid[] CpHeliSchoolEids { get; set; }

		/// <summary>
		/// 对口直升-政策文章
		/// </summary> 
		public DgAySchPcyFileDto CpHeliSchPcyFile { get; set; }

		/// <summary>
		/// 统筹入学-小学eids
		/// </summary> 
		public Guid[] OvSchoolEids { get; set; }

		/// <summary>
		/// 统筹入学-政策文章
		/// </summary> 
		public DgAySchPcyFileDto OvSchPcyFile { get; set; }

		/// <summary>
		/// 积分入学-积分
		/// </summary> 
		public double? JfPoints { get; set; }

		/// <summary>
		/// 积分入学-学部eids
		/// </summary> 
		public Guid[] JfSchoolEids { get; set; }

		/// <summary>
		/// 积分入学-政策文章
		/// </summary> 
		public DgAySchPcyFileDto JfSchPcyFile { get; set; }

		/// <summary>
		/// 找民办-民办小学eids
		/// </summary> 
		public Guid[] MbSchoolEids { get; set; }

		/// <summary>
		/// 所有的学部eids
		/// </summary> 
		public List<Guid> Eids { get; set; } = new List<Guid>();
	}

	/// <summary>政策文件</summary> 
	public class DgAySchPcyFileDto
	{
		public int Year { get; set; }
		/// <summary>政策文章</summary> 
		public string Title { get; set; }
		/// <summary>政策文章url</summary> 
		public string Url { get; set; }
	}
}
