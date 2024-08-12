using MediatR;
using Sxb.Infrastructure.Core.Extensions;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Commands
{
    public class PayDgAyOrderCommandHandler : IRequestHandler<PayDgAyOrderCommand>
    {
        IDgAyOrderRepository _repository;
        IMediator _mediator;
        public PayDgAyOrderCommandHandler(IDgAyOrderRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(PayDgAyOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetAsync(request.OrderId);
            if (order == null) throw new KeyNotFoundException($"找不到该订单。orderId={request.OrderId}");
            order.Pay(request.PayWay);
            bool udflag = await _repository.UpdateAsync(order, nameof(order.PayWay), nameof(order.Payment), nameof(order.Status), nameof(order.UpdateTime));
            if (!udflag) throw new System.Exception("更新订单失败。");
            await _mediator.DispatchDomainEventsAsync(order);
            return Unit.Value; ;
        }
    }
}
