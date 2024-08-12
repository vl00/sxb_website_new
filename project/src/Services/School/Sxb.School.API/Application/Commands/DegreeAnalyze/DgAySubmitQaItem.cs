using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.School.API.Application.Commands
{
    public class DgAySubmitQaItem
    {
        /// <summary>问题id</summary>
        public long Id { get; set; }
        /// <summary>
		/// 题型 1=单选 2=多选 3=单选计分 4=下拉计分 5=地址选择 6=地图定位 7=地区单选
		/// </summary> 
		public int Type { get; set; }

        /// <summary>
        /// 题型=单选|单选计分|下拉计分 的 用户选项
        /// </summary>
        public DgAySubmitQaAnsItem Item { get; set; } = null;
        /// <summary>
        /// 题型=多选 的 用户选项
        /// </summary>
        public DgAySubmitQaAnsItem[] Items { get; set; } = null;
        /// <summary>
        /// 题型=地址选择 的 用户选项
        /// </summary>
        public DgAySubmitQaAnsTy5Item Ty5Item { get; set; } = null;
        /// <summary>
        /// 题型=地图定位 的 用户选项
        /// </summary>
        public DgAySubmitQaAnsTy6Item Ty6Item { get; set; } = null;
        /// <summary>
        /// 题型=地区单选 的 用户选项
        /// </summary>
        public DgAySubmitQaAnsTy7Item Ty7Item { get; set; } = null;
    }

    public class DgAySubmitQaAnsItem
    {
        /// <summary>
        /// 选项id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
		/// 选项分数,当问题题型为计分类时使用
		/// </summary> 
		public double? Point { get; set; }
    }

    public class DgAySubmitQaAnsTy5Item
    {
        /// <summary>地区编码</summary>
        public long Area { get; set; }
        /// <summary>房产地址</summary>
        public string Address { get; set; }
    }

    public class DgAySubmitQaAnsTy6Item 
    {
        /// <summary>经度</summary>
        public double Lng { get; set; }
        /// <summary>纬度</summary>
        public double Lat { get; set; }
        /// <summary>城市.选择的经纬度所在的城市</summary>
        public string City { get; set; } // d+
        /// <summary>区.选择的经纬度所在的城市的区</summary>
        public string Area { get; set; } // d+
    }

    public class DgAySubmitQaAnsTy7Item
    {
        /// <summary>地区编码</summary>
        public long Area { get; set; }
    }
}
