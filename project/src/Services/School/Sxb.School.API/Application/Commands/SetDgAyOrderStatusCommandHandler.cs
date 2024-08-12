using MediatR;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Commands
{
    public class SetDgAyOrderStatusCommandHandler : IRequestHandler<SetDgAyOrderStatusCommand>
    {
        IDgAyOrderRepository _repository;
        public SetDgAyOrderStatusCommandHandler(IDgAyOrderRepository repository)
        {
            _repository = repository;
        }
        public async Task<Unit> Handle(SetDgAyOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetAsync(request.OrderId);
            if (order == null) throw new KeyNotFoundException($"找不到该订单。orderId={request.OrderId}");
            order.SetStatus(request.State);
            bool udflag = await _repository.UpdateAsync(order, nameof(order.Status), nameof(order.UpdateTime));
            if (!udflag) throw new System.Exception("更新订单失败。");
            return Unit.Value; ;
        }
    }
}
