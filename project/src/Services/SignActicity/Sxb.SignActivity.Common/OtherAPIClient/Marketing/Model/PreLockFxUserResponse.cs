using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.OtherAPIClient.Marketing.Model
{
    public class PreLockFxUserResponse
	{
		public Guid Id { get; set; }
		/// <summary> 
		/// 用户Id 
		/// </summary> 
		public Guid UserId { get; set; }

		/// <summary> 
		/// 父级Id 
		/// </summary> 
		public Guid? ParentUserId { get; set; }


		/// <summary> 
		/// 1正常 0删除
		/// </summary> 
		public int Status { get; set; }

		/// <summary> 
		///  
		/// </summary> 
		public DateTime Createime { get; set; }
		public DateTime CreateTime => Createime;
		public int channel { get; set; }

	}
}
