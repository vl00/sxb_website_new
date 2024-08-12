using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Domain.AggregateModels.DgAyUserQaPaperAggregate
{
	public  class DgAyUserQaPaper : Entity<Guid>, IAggregateRoot
	{



		/// <summary>
		/// 
		/// </summary> 
		public Guid UserId { get; private set; }


		/// <summary>
		/// 
		/// </summary> 
		public string Title { get; private set; }

		/// <summary>
		/// 分析类型
		/// </summary> 
		public byte Atype { get; private set; }

		/// <summary>
		/// 1=做题阶段，2=已分析出结果, 3=已解锁
		/// </summary> 
		public byte Status { get; private set; }

		/// <summary>
		/// 提交次数
		/// </summary> 
		public int? SubmitCount { get; private set; }

		/// <summary>
		/// 最后一次提交时间
		/// </summary> 
		public DateTime? LastSubmitTime { get; private set; }

		/// <summary>
		/// 分析出结果时间
		/// </summary> 
		public DateTime? AnalyzedTime { get; private set; }

		/// <summary>
		/// 1=免费解锁, 2=解锁x元, 3=解锁1元, 0=无结果变解锁
		/// </summary> 
		public byte? UnlockedType { get; private set; }

		/// <summary>
		/// 解锁时间
		/// </summary> 
		public DateTime? UnlockedTime { get; private set; }


		public void SetTitle(int times)
		{ 
			this.Title = $"{this.UnlockedTime:yyyy年MM月dd日}{(times <= 1 ? "" : $"第{times}次")}学位分析结果";
		}

		/// <summary>
		/// 解锁结果
		/// </summary>
		/// <param name="unlockedType">1=免费解锁, 2=解锁x元, 3=解锁1元, 0=无结果变解锁</param>
		public void UnLock(byte unlockedType)
		{
			this.Status = 3;
			this.UnlockedTime = DateTime.Now;
			this.UnlockedType = unlockedType;


		}

        public DgAyUserQaPaper(Guid id, Guid userId, string title, byte atype, byte status, int? submitCount, DateTime? lastSubmitTime, DateTime? analyzedTime, byte? unlockedType, DateTime? unlockedTime)
        {
            Id = id;
            UserId = userId;
            Title = title;
            Atype = atype;
            Status = status;
            SubmitCount = submitCount;
            LastSubmitTime = lastSubmitTime;
            AnalyzedTime = analyzedTime;
            UnlockedType = unlockedType;
            UnlockedTime = unlockedTime;
        }

    }
}
