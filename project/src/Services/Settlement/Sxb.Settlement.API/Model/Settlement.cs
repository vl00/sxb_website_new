using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sxb.Settlement.API.Services;

namespace Sxb.Settlement.API.Model
{
    public class Settlement
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNum { get; init; }

        /// <summary>
        /// 金额，单位元
        /// </summary>
        public decimal Amount { get; init; }


        /// <summary>
        /// 微信APPID
        /// </summary>
        public string WxAppId { get; init; }

        /// <summary>
        /// 微信OpenId
        /// </summary>
        public string WxOpenId { get; init; }

        public string BusinessSceneCode { get; init; }

        public string StatusCallBackUrl { get; init; }

        public SettlementStatus Status { get; private set; } = SettlementStatus.Waiting;

        public string Remark { get; private set; }


        public void StatusChange(SettlementStatus status,string remark)
        {
            if (status != this.Status)
            {
                this.Status = status;
                this.Remark = remark;
            }

        }


    }

   
    public enum SettlementStatus
    {
        Success = 1,
        Waiting = 2,
        Fail = 4,
    }
}
