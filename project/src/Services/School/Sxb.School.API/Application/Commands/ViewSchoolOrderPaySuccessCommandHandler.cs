using MediatR;
using Sxb.Infrastructure.Core.Extensions;
using Sxb.School.Domain.AggregateModels.ViewOrder;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Commands
{
    public class ViewSchoolOrderPaySuccessCommandHandler : IRequestHandler<ViewSchoolOrderPaySuccessCommand>
    {
        ISchoolViewOrderRepository _schoolViewOrderRepository;
        IMediator _mediator;
        public ViewSchoolOrderPaySuccessCommandHandler(ISchoolViewOrderRepository schoolViewOrderRepository, IMediator mediator)
        {
            _schoolViewOrderRepository = schoolViewOrderRepository;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(ViewSchoolOrderPaySuccessCommand request, CancellationToken cancellationToken)
        {
            var order = await _schoolViewOrderRepository.GetAsync(request.OrderId);
            if (order == null) throw new KeyNotFoundException($"找不到该订单。orderId={request.OrderId}");
            order.PaySuccess(request.PayWay, order.Amount);
            bool udflag = await _schoolViewOrderRepository.UpdateAsync(order, nameof(order.PayWay), nameof(order.Payment), nameof(order.Status), nameof(order.UpdateTime));
            if (!udflag) throw new System.Exception("更新订单失败。");
            await _mediator.DispatchDomainEventsAsync(order);
            return Unit.Value;
        }
    }
}
