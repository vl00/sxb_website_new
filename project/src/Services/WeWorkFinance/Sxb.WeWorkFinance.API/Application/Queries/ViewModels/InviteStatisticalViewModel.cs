using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Queries.ViewModels
{
    public class InviteStatisticalViewModel
    {
        public string UnionId { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 总数成员
        /// </summary>
        public string TotalUser { get; set; }
        /// <summary>
        /// 有效好友数
        /// </summary>
        public int ValidTotal { get; set; }
        /// <summary>
        /// 有效好友
        /// </summary>
        public string ValidUser { get; set; }
        /// <summary>
        /// 非有效好友数
        /// </summary>
        public int UnvalidTotal { get; set; }
        /// <summary>
        /// 非加群好友数
        /// </summary>
        public int NotJoinTotal { get; set; }
        /// <summary>
        /// 非加群好友
        /// </summary>
        public string NotJoinUser { get; set; }
        /// <summary>
        /// 非女性好友数
        /// </summary>
        public int NotladyTotal { get; set; }
        /// <summary>
        /// 非女性好友
        /// </summary>
        public string NotladyUser { get; set; }
        /// <summary>
        /// 之前已经加过群好友数
        /// </summary>
        public int BeforeJoinTotal { get; set; }
        /// <summary>
        /// 之前已经加过群好友
        /// </summary>
        public string BeforeJoinUser { get; set; }

    }
}
