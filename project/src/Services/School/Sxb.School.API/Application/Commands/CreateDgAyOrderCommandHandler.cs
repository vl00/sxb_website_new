using MediatR;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Commands
{
    public class CreateDgAyOrderCommandHandler : IRequestHandler<CreateDgAyOrderCommand, DgAyOrder>
    {
        IDgAyOrderRepository _repository;
        public CreateDgAyOrderCommandHandler(IDgAyOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<DgAyOrder> Handle(CreateDgAyOrderCommand request, CancellationToken cancellationToken)
        {
            DgAyOrder order = DgAyOrder.NewDraft(request.UserId,request.Termtyp);
            foreach (var productInfo in request.ProductInfos)
            {
                order.AddOrderDetail(DgAyOrderDetail.NewDraft(productInfo.ProductId, productInfo.productType, productInfo.ProductName, productInfo.ProductDesc, productInfo.Number, productInfo.UnitPrice, productInfo.OriginUnitPrice));
            }
            await _repository.AddAsync(order);
            return order;
        }
    }
}
