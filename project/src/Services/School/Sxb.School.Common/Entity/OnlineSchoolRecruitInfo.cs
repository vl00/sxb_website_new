using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 招生信息
    /// </summary>
    [Serializable]
    public class OnlineSchoolRecruitInfo
    {
        [JsonIgnore]
        [Identity(false)]
        public Guid ID { get; set; }
        [JsonIgnore]
        public Guid EID { get; set; }
        /// <summary>
        /// 招生信息类型
        /// <para>1.本校招生信息</para>
        /// <para>2.户籍生招生信息</para>
        /// <para>3.非户籍生积分入学</para>
        /// <para>4.分类招生简章</para>
        /// <para>5.社会公开招生简章</para>
        /// <para>6.自主招生</para>
        /// </summary>
        [Description("招生信息类型")]
        public int Type { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        [Description("年份")]
        public int Year { get; set; }
        /// <summary>
        /// 最小招生年龄
        /// </summary>
        [Description("最小招生年龄")]
        public string MinAge { get; set; }
        /// <summary>
        /// 最大招生年龄
        /// </summary>
        [Description("最大招生年龄")]
        public string MaxAge { get; set; }
        /// 招生人数
        /// </summary>
        [Description("招生人数")]
        public int? Quantity { get; set; }
        /// <summary>
        /// 招生总计划(人数)
        /// </summary>
        [Description("招生总计划(人数)")]
        public int? PlanQuantity { get; set; }
        /// <summary>
        /// 指标计划
        /// </summary>
        [Description("指标计划")]
        public int? QuotaPlanQuantity { get; set; }
        /// <summary>
        /// 招生对象
        /// </summary>
        [Description("招生对象")]
        public string Target { get; set; }
        /// <summary>
        /// 招生范围
        /// </summary>
        [Description("招生范围")]
        public string Score { get; set; }
        /// <summary>
        /// 招生比例
        /// </summary>
        [Description("招生比例")]
        public string Scale { get; set; }
        /// <summary>
        /// 招生简章
        /// </summary>
        [Description("招生简章")]
        public string Brief { get; set; }
        /// <summary>
        /// 派位小学
        /// </summary>
        [JsonIgnore]
        [Description("派位小学")]
        public string AllocationPrimary { get; set; }
        [Display(IsField = false)]
        public object AllocationPrimary_Obj
        {
            get
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(AllocationPrimary);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 对口小学
        /// </summary>
        [JsonIgnore]
        [Description("对口小学")]
        public string CounterpartPrimary { get; set; }
        [Display(IsField = false)]
        public object CounterpartPrimary_Obj
        {
            get
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(CounterpartPrimary);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 对口范围
        /// </summary>
        [Description("对口范围")]
        public string CounterpartScore { get; set; }
        /// <summary>
        /// 派位范围
        /// </summary>
        [Description("派位范围")]
        public string AllocationScore { get; set; }
        /// <summary>
        /// 积分入学指标及分值体系表图片URL
        /// </summary>
        [Description("积分入学指标及分值体系表图片URL")]
        public string IntegralImgUrl { get; set; }
        /// <summary>
        /// 积分入学入围分数线
        /// </summary>
        [Description("积分入学入围分数线")]
        public decimal? IntegralPassLevel { get; set; }
        /// <summary>
        /// 积分入学录取分数线
        /// </summary>
        [Description("积分入学录取分数线")]
        public decimal? IntegralAdmitLevel { get; set; }
        /// <summary>
        /// 申请费用
        /// </summary>
        [Description("申请费用")]
        public string ApplyCost { get; set; }
        /// <summary>
        /// 学费
        /// </summary>
        [Description("学费")]
        public string Tuition { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        [JsonIgnore]
        [Description("其他费用")]
        public string OtherCost { get; set; }
        [Display(IsField = false)]
        public object OtherCost_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(OtherCost);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 本区招生政策
        /// </summary>
        [JsonIgnore]
        [Description("本区招生政策")]
        public string AreaRecruitPlan { get; set; }
        [Display(IsField = false)]
        [JsonIgnore]
        public object AreaRecruitPlan_Obj
        {
            get
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(AreaRecruitPlan);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 招生计划
        /// </summary>
        [Description("招生计划")]
        public string Plan { get; set; }
        /// <summary>
        /// 招生条件
        /// </summary>
        [JsonIgnore]
        [Description("招生条件")]
        public string Requirement { get; set; }
        [Display(IsField = false)]
        public IEnumerable<string> Requirement_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Requirement))
                {
                    try
                    {
                        if (Requirement.Contains("；"))
                        {
                            return JsonConvert.DeserializeObject<IEnumerable<string>>($"[\"{Requirement.Replace("；", "\",\"")}\"]");
                        }
                        else
                        {
                            return JsonConvert.DeserializeObject<IEnumerable<string>>(Requirement);
                        }
                    }
                    catch { }
                }
                return null;
            }
        }
        /// <summary>
        /// 招生班数
        /// </summary>
        [Description("招生班数")]
        public int? ClassQuantity { get; set; }
        /// <summary>
        /// 招生材料
        /// </summary>
        [Description("招生材料")]
        public string Material { get; set; }
        /// <summary>
        /// 招生日程
        /// </summary>
        [Description("招生日程")]
        public string Schedule { get; set; }
        /// <summary>
        /// 学区范围
        /// </summary>
        [Description("学区范围")]
        public string SchoolScope { get; set; }
        /// <summary>
        /// 划片范围(对象)
        /// </summary>
        [JsonIgnore]
        [Description("划片范围(对象)")]
        public string ScribingScope { get; set; }
        [Display(IsField = false)]
        public IEnumerable<string[]> ScribingScope_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ScribingScope))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<IEnumerable<string[]>>(ScribingScope);
                    }
                    catch { }
                }
                return null;
            }
        }
        /// <summary>
        /// 划片范围(文字)
        /// </summary>
        [Description("划片范围(文字)")]
        public string ScribingScopeStr { get; set; }
        /// <summary>
        /// 统招生招生人数
        /// </summary>
        [Description("统招生招生人数")]
        public int? TZQuantity { get; set; }
        /// <summary>
        /// 调剂生招生人数
        /// </summary>
        [Description("调剂生招生人数")]
        public int? TJQuantity { get; set; }
        /// <summary>
        /// 项目班名称
        /// </summary>
        [Description("项目班名称")]
        public string ProjectClassName { get; set; }
        /// <summary>
        /// 普招计划人数
        /// </summary>
        [Description("普招计划人数")]
        public int? PZQuantity { get; set; }
        /// <summary>
        /// 联招计划人数
        /// </summary>
        [Description("联招计划人数")]
        public int? LZQuantity { get; set; }
        /// <summary>
        /// 中考分数控制线
        /// </summary>
        [Description("中考分数控制线")]
        public decimal? ZKFSKZX { get; set; }
        /// <summary>
        /// 派位小学EIDs
        /// </summary>
        [JsonIgnore]
        [Description("派位小学EIDs")]
        public string AllocationPrimaryEIDs { get; set; }
        [Display(IsField = false)]
        [JsonIgnore]
        public IEnumerable<Guid> AllocationPrimaryEIDs_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<Guid>>(AllocationPrimaryEIDs);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 对口小学EIDs
        /// </summary>
        [JsonIgnore]
        [Description("对口小学EIDs")]
        public string CounterpartPrimaryEIDs { get; set; }
        [Display(IsField = false)]
        [JsonIgnore]
        public IEnumerable<Guid> CounterpartPrimaryEIDs_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<Guid>>(CounterpartPrimaryEIDs);
                }
                catch { }
                return null;
            }
        }
        [JsonIgnore]
        public string ScribingScopeEIDs { get; set; }
        [Display(IsField = false)]
        [JsonIgnore]
        public IEnumerable<Guid> ScribingScopeEIDs_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<Guid>>(ScribingScopeEIDs);
                }
                catch { }
                return null;
            }
        }

        [JsonIgnore]
        public string ExtraItems { get; set; }
        [Display(IsField = false)]
        public IEnumerable<KeyValuePair<string, string>> ExtraItems_Obj
        {
            get
            {
                return ExtraItems.FromJsonSafe<IEnumerable<KeyValuePair<string, string>>>();
            }
        }
    }
}