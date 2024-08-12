using Dapper.Contrib.Extensions;
using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Runner.ImportAd.Model
{

	[Serializable]
	[Table("AdvertisingBase")]
	public partial class AdvertisingBase
	{
		/// <summary> 
		/// </summary> 
		[Identity]
		[Key]
		public int Id { get; set; }

		/// <summary> 
		/// </summary> 
		public string Title { get; set; }

		/// <summary> 
		/// 点击跳转链接 
		/// </summary> 
		public string Url { get; set; }

		/// <summary> 
		/// 图片链接 
		/// </summary> 
		public string PicUrl { get; set; }

		/// <summary> 
		/// </summary> 
		public int? Width { get; set; }

		/// <summary> 
		/// </summary> 
		public int? Height { get; set; }

		/// <summary> 
		/// </summary> 
		public DateTime? CreateTime { get; set; }

		/// <summary> 
		/// </summary> 
		public string Creator { get; set; }

		/// <summary> 
		/// </summary> 
		public DateTime? UpdateTime { get; set; }

		/// <summary> 
		/// </summary> 
		public string Updator { get; set; }

		/// <summary> 
		/// </summary> 
		public bool IsDelete { get; set; }

		/// <summary> 
		/// 0 未提交  1  上线  2 挂起 
		/// </summary> 
		public int Status { get; set; }

		public bool IsSpecial { get; set; }
	}


	[Serializable]
	[Table("AdvertisingSchedule")]
	public partial class AdvertisingSchedule
	{
		/// <summary> 
		/// </summary> 
		[ExplicitKey]
		public Guid Id { get; set; }

		/// <summary> 
		/// </summary> 
		public DateTime? UpTime { get; set; }

		/// <summary> 
		/// </summary> 
		public DateTime? ExpireTime { get; set; }

		/// <summary> 
		/// </summary> 
		public int? Location { get; set; }

		/// <summary> 
		/// </summary> 
		public DateTime? CreateTime { get; set; }

		/// <summary> 
		/// </summary> 
		public string Creator { get; set; }

		/// <summary> 
		/// </summary> 
		public DateTime? UpdateTime { get; set; }

		/// <summary> 
		/// </summary> 
		public string Updator { get; set; }

		/// <summary> 
		/// </summary> 
		public decimal Sort { get; set; }

		/// <summary> 
		/// 是否删除 
		/// </summary> 
		public bool IsDeleted { get; set; }


	}


	[Serializable]
	[Table("Ad_City_Schedule_R")]
	public partial class Ad_City_Schedule_R
	{
		/// <summary> 
		/// </summary> 
		[Identity]
		[Key]
		public int Id { get; set; }

		/// <summary> 
		/// 排期Id 
		/// </summary> 
		public Guid ScheduleId { get; set; }

		/// <summary> 
		/// 城市id 
		/// </summary> 
		public int CityId { get; set; }

		/// <summary> 
		/// 广告位广告 r_Id 
		/// </summary> 
		public int AdId { get; set; }

		/// <summary> 
		/// 是否优先  0 否 1 是 
		/// </summary> 
		public bool IsTop { get; set; }

		/// <summary> 
		/// 显示排序 
		/// </summary> 
		public int Sort { get; set; }

		/// <summary> 
		/// </summary> 
		public bool IsDeleted { get; set; }

		/// <summary> 
		/// </summary> 
		public DateTime CreateTime { get; set; }
		public DateTime UpdateTime { get; set; }

		/// <summary> 
		/// </summary> 
		public string Creator { get; set; }

		/// <summary> 
		/// 0 城市排期  1 学校排期 2 文章排期 
		/// </summary> 
		public byte DataType { get; set; }

		/// <summary> 
		/// 排期对象id 
		/// </summary> 
		public string DataId { get; set; }

		/// <summary> 
		/// 在其之前的城市排期数量 
		/// </summary> 
		public int BeforeCount { get; set; }
		public int LocationId { get; set; }
	}
}
