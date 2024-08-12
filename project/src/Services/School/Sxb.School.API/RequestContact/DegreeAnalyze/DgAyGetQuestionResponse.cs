using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sxb.School.API.Application.Queries.DegreeAnalyze;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sxb.School.API.RequestContact.DegreeAnalyze
{
    /// <summary>
    /// 获取题目s
    /// </summary>
    public class DgAyGetQuestionResponse
    {
        /// <summary>
        /// 第一题id
        /// </summary>
        public long Qid1st { get; set; } = 1;
        /// <summary>
        /// 问题s
        /// </summary>
        public List<DgAyQuestionVm> Ques { get; set; }
    }


}
