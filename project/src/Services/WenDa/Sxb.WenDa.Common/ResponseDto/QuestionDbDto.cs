using Sxb.WenDa.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class QuestionDbDto : Question
    {
        public Guid[] Eids { get; set; }
        public long[] TagIds { get; set; }
        public string[] TagNames { get; set; }
        public string CityName { get; set; }
        public string CategoryName { get; set; }
    }
}
