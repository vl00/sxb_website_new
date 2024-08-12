using System;
using System.Collections.Generic;

namespace Sxb.WeWorkFinance.API.Application.Queries.ViewModels
{
    public class ChatDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Msgid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tolist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Roomid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Msgtime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Msgtype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Contents { get; set; }

        public long Seq { get; set; }
    }
}
