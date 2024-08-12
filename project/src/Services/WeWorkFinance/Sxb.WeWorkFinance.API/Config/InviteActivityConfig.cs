using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Config
{
    public class InviteActivityConfig
    {
        public List<string> CorpTagId { get; set; }
        public DateTime ActivityEndTime { get; set; }
        public string WelcomeMessage { get; set; }
        public string WelcomeMessageGZ { get; set; }
        public string FirstSuccessInviteMessage { get; set; }
        public string SuccessInviteMessage { get; set; }
        public string PointLackMessage { get; set; }
        public string PointRewardAfterMessage { get; set; }
        public string TemplateMsgId { get; set; }
        public string TemplateMsgText { get; set; }

        public string MiniprogramAppid { get; set; }
    }
}
