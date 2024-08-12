using Sxb.Framework.Foundation;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sxb.School.Common.DTO
{
    public class DgAyQuestionOptionDto : DgAyQuestionOption
    {
        public long? NextQid { get; set; }
    }
}
