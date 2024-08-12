using MediatR;
using Sxb.School.Domain.AggregateModels.ViewOrder;
using System;

namespace Sxb.School.API.Application.Commands
{


    /// <summary>
    /// 创建查看全部学校详情的二维码，承包了下订单的职责
    /// </summary>
    public class CreateViewSchoolQRCodeCommand : IRequest<(SchoolViewOrder order,string qrcodeUrl)>
    {
        public Guid? UserId { get; set; }

        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid ExtId { get; set; }

        /// <summary>
        /// 扫码回调地址
        /// </summary>
        public string ScanCallBackUrl { get; set; }

        /// <summary>
        /// 1 ->免费模式
        /// 2 ->付费模式
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string FW { get; set; }



    }
}
