using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.School.API.Application.Commands
{
    /// <summary>
    /// 提交问卷(一次提交)
    /// </summary>
    public class DgAySubmitQaCmd : IRequest<DgAySubmitQaCmdResult>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        /// <summary>
        /// 按顺序每一题的提交参数
        /// </summary>
        public DgAySubmitQaItem[] Items { get; set; }
        /// <summary>
        /// 终端类型 1=h5 2=pc 3=小程序
        /// </summary>
        public int Termtyp { get; set; }
    }



    public class DgAySubmitQaCmdResult
    {
        /// <summary>
        /// 问卷(分析结果报告)id
        /// </summary>
        public Guid Qaid { get; set; }


    }
}
