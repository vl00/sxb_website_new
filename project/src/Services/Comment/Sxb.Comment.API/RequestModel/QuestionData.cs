using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Sxb.Comment.API.RequestModel
{
    public class QuestionData
    {
        [Required(ErrorMessage = "请输入问题")]
        public string Question { get; set; }
        public Guid Eid { get; set; }
        public List<string> Images { get; set; }
    }
}
