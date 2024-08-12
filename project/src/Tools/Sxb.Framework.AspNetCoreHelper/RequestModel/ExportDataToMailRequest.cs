using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Framework.AspNetCoreHelper.RequestModel
{
    public class ExportDataToMailRequest
    {
        public List<string> MainMails { get; set; }
        public List<string> CCMails { get; set; }
    }
}
