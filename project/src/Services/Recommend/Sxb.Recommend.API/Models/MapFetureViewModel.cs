using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Recommend.API.Models
{
    public class MapFetureViewModel
    {

        public string Id { get; set; }

        /// <summary>
        /// 分值
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public double Weight { get; set; }


        /// <summary>
        /// 特性别称
        /// </summary>
        public string Alias { get; set; }

    }
}
