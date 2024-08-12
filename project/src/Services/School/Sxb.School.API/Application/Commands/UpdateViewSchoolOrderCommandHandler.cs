using MediatR;
using Sxb.School.Domain.AggregateModels.ViewOrder;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Commands
{
    public class UpdateViewSchoolOrderCommandHandler : IRequestHandler<UpdateViewSchoolOrderCommand>
    {
        ISchoolViewOrderRepository _schoolViewOrderRepository;
        public UpdateViewSchoolOrderCommandHandler(ISchoolViewOrderRepository schoolViewOrderRepository)
        {
            _schoolViewOrderRepository = schoolViewOrderRepository;
        }

        public async Task<Unit> Handle(UpdateViewSchoolOrderCommand request, CancellationToken cancellationToken)
        {
           var order = await  _schoolViewOrderRepository.GetAsync(request.OrderId);
            order.SetAmount(request.Amount);
            bool udflag = await _schoolViewOrderRepository.UpdateAsync(order, nameof(order.Amount), nameof(order.UpdateTime));
            if (!udflag) throw new System.Exception("更新订单失败。");
            return Unit.Value;
        }
    }
}
