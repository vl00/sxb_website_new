using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.School.API.Application.Queries.DegreeAnalyze
{
    public class DgAyQaIsUnlockedVm
    {
        /// <summary>报告id</summary>
        public Guid Id { get; set; }
        /// <summary>
        /// true=已解锁 false=未解锁
        /// </summary>
        public bool IsUnlocked { get; set; }
    }

    public class DgAyQaResultVm0
    {
        /// <summary>报告id</summary>
        public Guid Id { get; set; }
        /// <summary>报告标题</summary>
        public string Title { get; set; }
        /// <summary>
        /// 分析类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 分析类型(中文)
        /// </summary>
        public string TypeDesc { get; set; }
        /// <summary>
        /// true=已解锁 false=未解锁
        /// </summary>
        public bool IsUnlocked { get; set; }

        /// <summary>用户答题内容</summary>
        public DgAyUserQaContentVm QaContent { get; set; } = default;

        /// <summary>
        /// true=是我的 false=不是我的 null=不判断是否我的
        /// </summary>
        public bool? IsMy { get; set; }
    }

    /// <summary>
    /// 分析报告（含未解锁和已解锁）
    /// </summary>
    public class DgAyQaResultVm : DgAyQaResultVm0
    {
        /// <summary>
        /// 分析结果内容-对口入学<br/>
        /// type=1时使用
        /// </summary>
        public DgAyQaResultVm_ResultTy1 ResultTy1 { get; set; } = null;
        /// <summary>
        /// 分析结果内容-统筹入学<br/>
        /// type=2时使用
        /// </summary>
        public DgAyQaResultVm_ResultTy2 ResultTy2 { get; set; } = null;
        /// <summary>
        /// 分析结果内容-积分入学<br/>
        /// type=3时使用
        /// </summary>
        public DgAyQaResultVm_ResultTy3 ResultTy3 { get; set; } = null;
        /// <summary>
        /// 分析结果内容-查找心仪民办小学<br/>
        /// type=4时使用
        /// </summary>
        public DgAyQaResultVm_ResultTy4 ResultTy4 { get; set; } = null;
    }

    public class DgAyQaResultVm_ResultTy1
    {
        /// <summary>
        /// 对口入学小学
        /// </summary>
        public DgAySchoolItemVm CpSchool { get; set; }
        /// <summary>对口入学小学-政策文件</summary>
        public DgAySchPcyFileVm CpPcyFile { get; set; }

        /// <summary>
        /// 对口直升-学校list
        /// </summary>
        public List<DgAySchoolItemVm> CpHeliSchools { get; set; } = null;
        /// <summary>对口直升-政策文件</summary>
        public DgAySchPcyFileVm CpHeliPcyFile { get; set; } = null;

        /// <summary>
        /// 电脑派位-学校list
        /// </summary>
        public List<DgAySchoolItemVm> CpPcAssignSchools { get; set; } = null;
        /// <summary>电脑派位-政策文件</summary>
        public DgAySchPcyFileVm CpPcAssignPcyFile { get; set; } = null;
    }

    public class DgAyQaResultVm_ResultTy2
    {
        /// <summary>
        /// 统筹入学-学校list
        /// </summary>
        public List<DgAySchoolItemVm> OvSchools { get; set; } = null;
        /// <summary>统筹入学-政策文件</summary>
        public DgAySchPcyFileVm OvPcyFile { get; set; } = null;
    }

    public class DgAyQaResultVm_ResultTy3
    {
        /// <summary>
        /// 积分入学-积分
        /// </summary>
        public double JfPoints { get; set; }

        /// <summary>
        /// 积分入学-学校list
        /// </summary>
        public List<DgAySchoolItemVm> JfSchools { get; set; } = null;
        /// <summary>积分入学-政策文件</summary>
        public DgAySchPcyFileVm JfPcyFile { get; set; } = null;
    }

    public class DgAyQaResultVm_ResultTy4
    {
        /// <summary>
        /// 找民办-学校list
        /// </summary>
        public List<DgAySchoolItemVm> MbSchools { get; set; } = null;
    }

    public class DgAySchoolItemVm
    {
        [JsonIgnore]
        public Guid Eid { get; set; }
        /// <summary>学部短id</summary>
        public string Eid_s { get; set; }
        /// <summary>学校名</summary>
        public string Schname { get; set; }
        /// <summary>学部名</summary>
        public string Extname { get; set; }
        /// <summary>地址</summary>
        public string Address { get; set; }

        /// <summary>标签</summary>
        public List<string> Tags { get; set; }

        [JsonIgnore]
        public double? Score { get; set; }
        /// <summary>
        /// 分数等级
        /// </summary>
        public string Str_Score
        {
            get
            {
                if (Score.HasValue)
                {
                    if (Score.Value >= 90)
                    {
                        return "A+";
                    }
                    else if (Score.Value >= 80 && Score.Value < 90)
                    {
                        return "A";
                    }
                    else if (Score.Value >= 70 && Score.Value < 80)
                    {
                        return "B";
                    }
                    else if (Score.Value >= 60 && Score.Value < 70)
                    {
                        return "C";
                    }
                    else
                    {
                        return "D";
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 招生方式
        /// </summary>
        public List<string> RecruitWay { get; set; }

        /// <summary>
        /// 积分入学-年份
        /// </summary>
        public int? JfYear { get; set; }
        /// <summary>
        /// 积分入学-录取分数线
        /// </summary>
        public double? JfScoreLine { get; set; }
    }

    /// <summary>政策文件</summary>
    public class DgAySchPcyFileVm
    {
        /// <summary>政策文件年份</summary>
        public int Year { get; set; }
        /// <summary>政策文件</summary>
        public string Title { get; set; }
        /// <summary>政策文件url</summary>
        public string Url { get; set; }
    }

}
