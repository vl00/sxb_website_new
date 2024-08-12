using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 学部其他信息
    /// </summary>
    [Serializable]
    public class SchoolOverViewInfo
    {
        [Identity(false)]
        [JsonIgnore]
        public Guid ID { get; set; }
        public Guid EID { get; set; }
        public Guid? SID { get; set; }
        /// <summary>
        /// 是否有校车
        /// </summary>
        [Description("是否有校车")]
        public bool HasSchoolBus { get; set; }
        /// <summary>
        /// 招生方式
        /// </summary>
        [JsonIgnore]
        [Description("招生方式")]
        public string RecruitWay { get; set; }
        [Display(IsField = false)]
        public object RecruitWay_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(RecruitWay);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 学校公众号名称
        /// </summary>
        [Description("学校公众号名称")]
        public string OAName { get; set; }
        /// <summary>
        /// 学校公众号appid
        /// </summary>
        [Description("学校公众号appid")]
        public string OAAppID { get; set; }
        /// <summary>
        /// 学校小程序名称
        /// </summary>
        [Description("学校小程序名称")]
        public string MPName { get; set; }
        /// <summary>
        /// 学校小程序appid
        /// </summary>
        [Description("学校小程序appid")]
        public string MPAppID { get; set; }
        /// <summary>
        /// 学校视频号名称
        /// </summary>
        [Description("学校视频号名称")]
        public string VAName { get; set; }
        /// <summary>
        /// 学校视频号appid
        /// </summary>
        [Description("学校视频号appid")]
        public string VAAppID { get; set; }
        /// <summary>
        /// 学校公众号帐号
        /// </summary>
        [Description("学校公众号帐号")]
        public string OAAccount { get; set; }
        /// <summary>
        /// 小程序帐号
        /// </summary>
        [Description("小程序帐号")]
        public string MPAccount { get; set; }
        /// <summary>
        /// 视频号帐号
        /// </summary>
        [Description("视频号帐号")]
        public string VAAccount { get; set; }
        /// <summary>
        /// 学部获得的认证
        /// </summary>
        [JsonIgnore]
        [Description("学部获得的认证")]
        public string Certifications { get; set; }
        [Display(IsField = false)]
        public object Certifications_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Certifications))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<dynamic>(Certifications);
                    }
                    catch { }
                }
                return null;
            }
        }
    }
}