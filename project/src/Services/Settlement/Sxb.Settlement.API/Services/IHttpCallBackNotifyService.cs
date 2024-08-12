using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Services
{
    public interface IHttpCallBackNotifyService
    {
        Task NotifySettlementStatus(string url, SettlementStatusMessage   settlementStatusMessage);

        Task NotifySettlementRefundSuccess(string url, SettlementRefundSuccessMessage settlementRefundMessage);

    }
}
