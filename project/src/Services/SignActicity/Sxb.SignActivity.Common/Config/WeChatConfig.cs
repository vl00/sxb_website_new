using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.Config
{
    public class WeChatConfig
    {
        public MpConfig MiniProgramOrg { get; set; }

        public MsgTemplate MsgTemplates { get; set; }




        public class MsgTemplate
        {
           public TemplateConfig SignIn { get; set; }
        }

        public class MpConfig
        {
            public string AppID { get; set; }
        }

        public class TemplateConfig
        {
            public string TemplateId { get; set; }
            public string PagePath { get; set; }
        }
    }
}
