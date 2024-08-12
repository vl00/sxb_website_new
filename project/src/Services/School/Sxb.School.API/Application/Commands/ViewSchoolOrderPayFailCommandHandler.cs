using MediatR;
using Sxb.School.Domain.AggregateModels.ViewOrder;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Commands
{
    public class ViewSchoolOrderPayFailCommandHandler : IRequestHandler<ViewSchoolOrderPayFailCommand>
    {
        ISchoolViewOrderRepository _schoolViewOrderRepository;
        IMediator _mediator;
        public ViewSchoolOrderPayFailCommandHandler(ISchoolViewOrderRepository schoolViewOrderRepository, IMediator mediator)
        {
            _schoolViewOrderRepository = schoolViewOrderRepository;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(ViewSchoolOrderPayFailCommand request, CancellationToken cancellationToken)
        {
            var order = await _schoolViewOrderRepository.GetAsync(request.OrderId);
            if (order == null) throw new KeyNotFoundException($"找不到该订单。orderId={request.OrderId}");
            order.PayFail(request.PayWay);
            bool udflag = await _schoolViewOrderRepository.UpdateAsync(order, nameof(order.PayWay), nameof(order.Status), nameof(order.UpdateTime));
            if (!udflag) throw new System.Exception("更新订单失败。");
            return Unit.Value;
        }
    }
}
