using MediatR;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Commands
{
    public class SetDgAyOrderUserCommandHandler : IRequestHandler<SetDgAyOrderUserCommand, bool>
    {

        IDgAyOrderRepository _repository;
        public SetDgAyOrderUserCommandHandler(IDgAyOrderRepository repository)
        {
            _repository = repository;
        }


        public async Task<bool> Handle(SetDgAyOrderUserCommand request, CancellationToken cancellationToken)
        {

            var order = await _repository.GetAsync(request.OrderId);
            if (order == null) throw new KeyNotFoundException("找不到该订单");
            bool flag = order.SetUser(request.UserId);
            if (flag)
                return await _repository.UpdateAsync(order, nameof(order.UserId), nameof(order.UpdateTime));
            return flag;
        }
    }
}
