using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class CityDto
    {
        /// <summary>城市编码</summary>
        public long Id { get; set; }
        /// <summary>城市名</summary>
        public string Name { get; set; }
        /// <summary>城市名简拼</summary>
        public string ShortName { get; set; }
        /// <summary>是否开通问答广场</summary>
        public bool IsOpen { get; set; }
    }
}
