using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Queries.UserSignInInfo
{
    public record NotifyUserInfoViewModel
    {
        public Guid UserId { get; set; }

        public int ContinueDays { get; set; }

        public DateTime? LastSignDate { get; set; }

        /// <summary>
        /// 第几天
        /// </summary>
        public int TheDay => ContinueDays % 7;

        public bool IsSignToday => LastSignDate != null && LastSignDate.Value.Date == DateTime.Now.Date;

    }
}
