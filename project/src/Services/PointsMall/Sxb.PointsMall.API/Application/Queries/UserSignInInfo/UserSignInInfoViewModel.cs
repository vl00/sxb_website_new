using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Queries.UserSignInInfo
{
    public record UserSignInInfo
    {

        public bool EnableNotify { get; set; }

        public int ContinueDays { get; set; }

        public DateTime? LastSignDate { get; set; }

        /// <summary>
        /// 第几天
        /// </summary>
        public int TheDay
        {
            get
            {
                if (LastSignDate == null || (DateTime.Now.Date - LastSignDate.Value.Date).TotalDays > 1 || ContinueDays == 0)
                    return 1;
                if (this.IsSignToday)
                {
                    return ((ContinueDays - 1) % 7) + 1;

                }
                else
                {
                    return (ContinueDays % 7) + 1;

                }
            }
        }

        public bool IsSignToday => LastSignDate != null && LastSignDate.Value.Date == DateTime.Now.Date;



    }
}
