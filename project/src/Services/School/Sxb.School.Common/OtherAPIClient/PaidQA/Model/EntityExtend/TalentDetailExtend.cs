using Sxb.Framework.Foundation;
using Sxb.School.Common.OtherAPIClient.PaidQA.Model.Entity;
using System;
using System.Collections.Generic;

namespace Sxb.School.Common.OtherAPIClient.PaidQA.Model.EntityExtend
{
    public class TalentDetailExtend : TalentSetting
    {
        /// <summary>
        /// 擅长领域
        /// </summary>
        public IEnumerable<RegionType> TalentRegions { get; set; }
        /// <summary>
        /// 学段
        /// </summary>
        public IEnumerable<Grade> TalentGrades { get; set; }
        /// <summary>
        /// 达人昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 头像URL
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// 达人认证名称
        /// </summary>
        public string AuthName { get; set; }
        /// <summary>
        /// 达人介绍
        /// </summary>
        public string TelentIntroduction { get; set; }
        /// <summary>
        /// 认证等级名称
        /// </summary>
        public string TalentLevelName { get; set; }
        /// <summary>
        /// 平均评分
        /// </summary>
        public double AvgScore { get; set; }
        /// <summary>
        /// 六小时回复率
        /// </summary>
        public double SixHourReplyPercent { get; set; }
        /// <summary>
        /// 指定用户是否关注该达人
        /// </summary>
        public bool IsFollowed { get; set; }
        /// <summary>
        /// 达人类型(0.个人 | 1.机构)
        /// </summary>
        public int TalentType { get; set; }

        /// <summary>
        /// 详情背景图
        /// </summary>
        public IEnumerable<string> Covers
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(JA_Covers)) JA_Covers.FromJson<IEnumerable<string>>();
                }
                catch { }
                return null;
            }
        }

        public Guid? SchoolExtId { get; set; }
    }
}
