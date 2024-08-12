﻿using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    /// <summary>
    /// 学校图片分类
    /// </summary>
    public enum SchoolImageType
    {
        Unkonw = 0,
        /// <summary>
        /// 学校荣誉
        /// </summary>
        [Description("学校荣誉")]
        SchoolHonor = 1,

        /// <summary>
        /// 学生荣誉
        /// </summary>
        [Description("学生荣誉")]
        StudentHonor = 2,

        /// <summary>
        /// 校长风采
        /// </summary>
        [Description("校长风采")]
        Principal = 3,

        /// <summary>
        /// 教师风采
        /// </summary>
        [Description("教师风采")]
        Teacher = 4,

        /// <summary>
        /// 硬件设施
        /// </summary>
        [Description("硬件设施")]
        Hardware = 5,

        /// <summary>
        /// 社团活动
        /// </summary>
        [Description("社团活动")]
        CommunityActivities = 6,

        /// <summary>
        /// 各年级课程表
        /// </summary>
        [Description("各年级课程表")]
        GradeSchedule = 7,

        /// <summary>
        /// 作息时间表
        /// </summary>
        [Description("作息时间表")]
        Schedule = 8,

        /// <summary>
        /// 校车路线
        /// </summary>
        [Description("校车路线")]
        Diagram = 9,

        /// <summary>
        /// 学校品牌
        /// </summary>
        [Description("学校品牌")]
        SchoolBrand = 10
    }
}
