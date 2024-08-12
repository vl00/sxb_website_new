using Sxb.WenDa.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class SubjectDbDto : QaSubject
    {
        public long[] CategoryIds { get; set; }
        public string[] CategoryNames { get; set; }
        public long[] TagIds { get; set; } = null;
        public string[] TagNames { get; set; } = null;
    }
}
