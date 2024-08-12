using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.OtherAPIClient.Marketing.Model
{
    public class FxUserResponse
	{
		/// <summary> 
		/// 用户Id 
		/// </summary> 
		public Guid UserId { get; set; }

		/// <summary> 
		/// 父级Id 
		/// </summary> 
		public Guid? ParentUserId { get; set; }

		/// <summary> 
		/// 工资系数 
		/// </summary> 
		public decimal BonusRate { get; set; }

		/// <summary> 
		/// 用户角色  0 无  1 粉丝  2 普通顾问  3高级顾问
		/// </summary> 
		public int Role { get; set; }

		/// <summary> 
		/// 成为顾问时间 
		/// </summary> 
		public DateTime? BecomeConsultantTime { get; set; }

		/// <summary> 
		/// 成为高级顾问时间 
		/// </summary> 
		public DateTime? BecomeHighConsultantTime { get; set; }

		/// <summary> 
		/// 加入团队时间  有ParentUserId的时间 
		/// </summary> 
		public DateTime? JoinTime { get; set; }
	}
}
