using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.School.API.Application.Queries.DegreeAnalyze
{
    /// <summary>用户答题内容</summary>
    public class DgAyUserQaContentVm
    {
        public List<DgAyQaQuesItemVm> Ques { get; set; }
    }

    /// <summary>
    /// 用户答题每题内容
    /// </summary>
    public class DgAyQaQuesItemVm
    { 
        public long? Id { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// 题型 1=单选 2=多选 3=单选计分 4=下拉计分 5=地址选择 6=地图定位 7=地区单选
        /// </summary> 
        public int Type { get; set; }
        /// <summary>
        /// 计分类型时, 题目的分数
        /// </summary>
        public double? Score { get; set; }

        /// <summary>
        /// 题型=单选|多选|单选计分|下拉计分 的 用户选项
        /// </summary>
        public List<DgAyQaOptItemVm> Opts { get; set; }
        /// <summary>
        /// 题型=地址选择 的 用户选项
        /// </summary>
        public DgAyQaOptItemTy5Vm OptTy5 { get; set; }
        /// <summary>
        /// 题型=地图定位 的 用户选项
        /// </summary>
        public DgAyQaOptItemTy6Vm OptTy6 { get; set; }
        /// <summary>
        /// 题型=地区单选 的 用户选项
        /// </summary>
        public List<DgAyQaOptItemTy7Vm> OptTy7 { get; set; }
    }

    public class DgAyQaOptItem
    {
        /// <summary>选项id</summary>
        public long? Id { get; set; }
        public string Title { get; set; }
        /// <summary>是否选中</summary>
        public bool Selected { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FindField { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FindFieldFw { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FindFieldFwJx { get; set; }
    }

    public class DgAyQaOptItemVm : DgAyQaOptItem
    {
        /// <summary>
		/// 选项分数,当问题题型为计分类时使用
		/// </summary> 
		public double? Point { get; set; }
    }

    public class DgAyQaOptItemTy5Vm
    {
        /// <summary>区</summary>
        public List<DgAyQaOptItem> Items1 { get; set; }
        /// <summary>地址</summary>
        public List<DgAyQaOptItem> Items2 { get; set; }
    }

    public class DgAyQaOptItemTy6Vm
    {
        /// <summary>经度</summary>
        public double Lng { get; set; }
        /// <summary>纬度</summary>
        public double Lat { get; set; }
        /// <summary>城市</summary>
        public string City { get; set; }
        /// <summary>区</summary>
        public string Area { get; set; }
    }

    public class DgAyQaOptItemTy7Vm : DgAyQaOptItem
    {
        /// <summary>地区编码</summary>
        public long Area { get; set; }
    }
}
