using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.Foundation
{
    public class SignConstantValue
    {
        /// <summary>
        /// 签到最小金额
        /// </summary>
        public static decimal SignAmount { get; set; } = 45;

        /// <summary>
        /// 邀请入群开始时间
        /// </summary>
        public static DateTime InviteStartTime { get; set; } = new DateTime(DateTime.Now.Year, 10, 29);
    }
}
