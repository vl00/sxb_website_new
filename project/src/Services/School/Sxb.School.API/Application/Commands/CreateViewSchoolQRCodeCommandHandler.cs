using MediatR;
using Sxb.School.API.Application.Models;
using Sxb.School.API.Infrastructures.Services;
using Sxb.School.Domain.AggregateModels.ViewOrder;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Commands
{


    /// <summary>
    /// 创建查看全部学校详情的二维码，承包了下订单的职责
    /// </summary>
    public class CreateViewSchoolQRCodeCommandHandler : IRequestHandler<CreateViewSchoolQRCodeCommand, (SchoolViewOrder order, string qrcodeUrl)>
    {

        ISchoolViewOrderRepository _schoolViewOrderRepository;
        IWeChatGatewayService _weChatGatewayService;
        public CreateViewSchoolQRCodeCommandHandler(ISchoolViewOrderRepository schoolViewOrderRepository
            , IWeChatGatewayService weChatGatewayService)
        {
            _schoolViewOrderRepository = schoolViewOrderRepository;
            _weChatGatewayService = weChatGatewayService;
        }

        public async Task<(SchoolViewOrder order, string qrcodeUrl)> Handle(CreateViewSchoolQRCodeCommand request, CancellationToken cancellationToken)
        {
            //创建订单 默认是一块。
            ViewSchoolGoodsInfo viewSchoolGoodsInfo;
            if (request.Mode == 1)
            {
                viewSchoolGoodsInfo = new ViewSchoolGoodsInfo(request.ExtId, 0);
            }
            else {
                viewSchoolGoodsInfo = new ViewSchoolGoodsInfo(request.ExtId, Configs.ViewSchoolUnitPrice);
            }
            SchoolViewOrder order = SchoolViewOrder.NewDraft(request.UserId, viewSchoolGoodsInfo);
            await _schoolViewOrderRepository.AddAsync(order);
            //生成关联订单的场景二维码。
            ViewSchoolScanCallBackAttachData attachData = new ViewSchoolScanCallBackAttachData()
            {
                OrderId = order.Id,
                Mode = request.Mode,
                FW = request.Mode == 1 ? $"viewpayschool_free_{request.ExtId}" : request.Mode == 2 ? $"viewpayschool_pay_{request.ExtId}" : ""

            };
            string qrcodeImgUrl = await _weChatGatewayService.GetSenceQRCode(new Infrastructures.Services.Models.GetSenceQRCodeRequest()
            {
                app = 0,
                callBackUrl = request.ScanCallBackUrl,
                expireSecond = (int)(order.ExpireTime - DateTime.Now).TotalSeconds,
                attach = Newtonsoft.Json.JsonConvert.SerializeObject(attachData),
                fw = request.FW 
            });

            return (order, qrcodeImgUrl);
        }
    }
}
