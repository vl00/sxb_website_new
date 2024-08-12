using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.School.API.Application.Queries.DegreeAnalyze
{
    public class DgAyQuestionVm
    {
        /// <summary>
        /// 问题id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 题目
        /// </summary>
        public string Title { get; set; }
        /// <summary>
		/// 题型 1=单选 2=多选 3=单选计分 4=下拉计分 5=地址选择 6=地图定位 7=地区单选
		/// </summary> 
		public byte Type { get; set; }

        /// <summary>
        /// (单选多选下拉)选项s 对于某些类型会为null
        /// </summary>
        public List<DgAyQuesOptionVm> Opts { get; set; }

        /// <summary>
        /// 地址选择-选项s 对于其他类型会为null
        /// </summary>
        public DgAyQuesOptionTy5Vm Ty5Opts { get; set; } = null;

        /// <summary>
        /// 地区单选-选项s 对于其他类型会为null
        /// </summary>
        public List<DgAyQuesOptionTy7Vm_Item1> Ty7Opts { get; set; } = null;

        /// <summary>
        /// 下一条问题id. <br/>
        /// 此值不为null时,表示题跳题; <br/>
        /// 为null时,表示选项跳题,用opts里的nextQid <br/>
        /// 优先使用选项跳题
        /// </summary>
        public long? NextQid { get; set; }
        /// <summary>
        /// 是否是最后一题
        /// </summary>
        public bool IsLast
        {
            get
            {                
                if (NextQid != null) return false;
                var b1 = (Opts?.Count() ?? 0) < 1 || !Opts.Any(_ => _.NextQid != null);
                var b7 = (Ty7Opts?.Count() ?? 0) < 1 || !Ty7Opts.Any(_ => _.NextQid != null);
                return b1 && b7;
            }
        }
        /// <summary>
        /// 是否是第一题
        /// </summary>
        [JsonIgnore]
        public bool Is1st { get; set; }

        /// <summary>
        /// 题型=多选 时最多可选多少选项 <br/>
        /// 此值null时没限制
        /// </summary>
        public int? MaxSelected { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FindField { get; set; }
    }

    /// <summary>
    /// (单选多选下拉)选项
    /// </summary>
    public class DgAyQuesOptionVm
    {
        /// <summary>
        /// 选项id 
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 所属问题id
        /// </summary>
        public long Qid { get; set; }
        /// <summary>
		/// 选项名
		/// </summary> 
		public string Title { get; set; }
        /// <summary>
		/// 选项分数,当问题题型为计分类时使用
		/// </summary> 
		public double? Point { get; set; }

        /// <summary>
        /// 下一条问题id. <br/>
        /// 此值不为null时,表示选项跳到题; <br/>
        /// 为null时,表示题跳题,用问题实体里的nextQid <br/>
        /// 优先使用选项跳题
        /// </summary>
        public long? NextQid { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FindField { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FindFieldFw { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FindFieldFwJx { get; set; }
    }

    public class DgAyQuesOptionTy5Vm
    {
        /// <summary>区</summary>
        public List<DgAyQuesOptionTy5Vm_Item1> Items1 { get; set; }
        /// <summary>地址</summary>
        public List<DgAyQuesOptionTy5Vm_Item2> Items2 { get; set; }
    }

    public class DgAyQuesOptionTy5Vm_Item1
    {
        /// <summary>选项id</summary>
        public long? Id { get; set; }
        public string Title { get; set; }
    }

    public class DgAyQuesOptionTy5Vm_Item2
    {
        public string Title { get; set; }
    }

    public class DgAyQuesOptionTy7Vm_Item1
    {
        /// <summary>选项id</summary>
        public long? Id { get; set; }
        /// <summary>地区</summary>
        public string Title { get; set; }
        /// <summary>地区编码</summary>
        public long Area { get; set; }

        /// <summary>
        /// 下一条问题id. <br/>
        /// 此值不为null时,表示选项跳到题; <br/>
        /// 为null时,表示题跳题,用问题实体里的nextQid <br/>
        /// 优先使用选项跳题
        /// </summary>
        public long? NextQid { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FindField { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FindFieldFw { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FindFieldFwJx { get; set; }
    }
}
