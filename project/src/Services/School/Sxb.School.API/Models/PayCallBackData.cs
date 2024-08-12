using System;
using System.ComponentModel;

namespace Sxb.School.API.Models
{
    public class PayCallBackData
    {
        public string OrderNo { get; set; }
        public Guid OrderId { get; set; }
        public PayStatusEnum PayStatus { get; set; }
        public DateTime AddTime { get; set; }
        public enum PayStatusEnum
        {

            [Description("待支付")]
            InProcess = 0,
            [Description("成功")]
            Success = 1,
            [Description("失败")]
            Fail = -1
        }
    }
}
