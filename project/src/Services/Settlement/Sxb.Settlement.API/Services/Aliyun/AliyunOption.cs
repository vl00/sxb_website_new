using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Services.Aliyun
{
    public class AliyunOption
    {
        public readonly static string AliyunConfig = "AliyunConfig";
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public string AppCode { get; set; }

    }
}
