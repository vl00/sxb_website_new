namespace Sxb.School.API.Application.Commands
{
    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;
    using Sxb.School.Domain.AggregateModels.ViewOrder;
    using System.Collections.Generic;

    public class SetUserForViewSchoolOrderCommandHandler : IRequestHandler<SetUserForViewSchoolOrderCommand, SchoolViewOrder>
    {
        ISchoolViewOrderRepository _viewOrderRepository;
        public SetUserForViewSchoolOrderCommandHandler(ISchoolViewOrderRepository viewOrderRepository)
        {
            _viewOrderRepository = viewOrderRepository;
        }
        public async Task<SchoolViewOrder> Handle(SetUserForViewSchoolOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _viewOrderRepository.GetAsync(request.OrderId);
            if (order == null) throw new KeyNotFoundException("找不到该订单");
            bool flag = order.SetUser(request.UserId);
            if (flag)
                await _viewOrderRepository.UpdateAsync(order, nameof(order.UserId), nameof(order.UpdateTime));
            return order;
        }
    }
}
