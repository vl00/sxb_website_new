using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.GaoDeng
{
    public class GaoDengOption
    {
        public readonly static string GaoDengConfig = "GaoDengConfig";
        public string BaseUrl { get; init; }

        public string AppKey { get; init; }

        public string AppSecret { get; init; }

        public string AESIV { get; init; }

        public string H5ToolUrl { get; init; }

        public string SettlementStatusCallBack { get; init; }

        public string SettilementRefundCallBack { get; init; }
    }
}
