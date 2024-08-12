using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Sxb.School.Common.Consts
{
    public static class CacheKeys
    {
        /// <summary>问题s+选项s+下一题</summary>
        public const string DgAyQuestions = "DegreeAnalyze:questions";
        /// <summary>限制用户提交答卷频率</summary>
        public const string DgAyXzUserSubmitQa = "DegreeAnalyze:xz_user_submit_qa:u_{0}";
    }
}
