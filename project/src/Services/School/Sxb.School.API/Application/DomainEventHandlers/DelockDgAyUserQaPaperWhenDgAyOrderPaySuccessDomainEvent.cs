using Sxb.Domain;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using Sxb.School.Domain.AggregateModels.DgAyUserQaPaperAggregate;
using Sxb.School.Domain.AggregateModels.ViewPermission;
using Sxb.School.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.DomainEventHandlers
{
    public class DelockDgAyUserQaPaperWhenDgAyOrderPaySuccessDomainEvent : IDomainEventHandler<DgAyOrderPaySuccessDomainEvent>
    {
        IDgAyOrderRepository _dgAyOrderRepository;
        IDgAyUserQaPaperRepository _dgAyUserQaPaperRepository;
        ISchoolViewPermissionRepository _schoolViewPermissionRepository;

        public DelockDgAyUserQaPaperWhenDgAyOrderPaySuccessDomainEvent(IDgAyOrderRepository dgAyOrderRepository
            , IDgAyUserQaPaperRepository dgAyUserQaPaperRepository
            , ISchoolViewPermissionRepository schoolViewPermissionRepository)
        {
            _dgAyOrderRepository = dgAyOrderRepository;
            _dgAyUserQaPaperRepository = dgAyUserQaPaperRepository;
            _schoolViewPermissionRepository = schoolViewPermissionRepository;
        }

        public async Task Handle(DgAyOrderPaySuccessDomainEvent notification, CancellationToken cancellationToken)
        {
            var orderDetails = await _dgAyOrderRepository.GetDgAyOrderDetailsAsync(notification.Order.Id);
            foreach (var orderDetail in orderDetails)
            {
                if (orderDetail.ProductType == DgAyProductType.DgAyResult)
                {
                    //解锁分析结果
                    var paper = await _dgAyUserQaPaperRepository.GetAsync(orderDetail.ProductId);
                    if (paper == null)
                        throw new KeyNotFoundException($"找不到Paper,id={orderDetail.ProductId}");
                    if (notification.Order.Payment == 0)
                        paper.UnLock(1);
                    else if (notification.Order.Payment == Configs.DgAyResultUnitPrice)
                        paper.UnLock(2);
                    else if (notification.Order.Payment > Configs.DgAyResultUnitPrice)
                        paper.UnLock(3);
                    else
                        break;
                    await _dgAyUserQaPaperRepository.UpdateAsync(paper, nameof(paper.UnlockedType), nameof(paper.UnlockedTime), nameof(paper.Status));
                    var times = await _dgAyUserQaPaperRepository.GetTimesAsync(paper.Id);
                    paper.SetTitle(times.Value);
                    await _dgAyUserQaPaperRepository.UpdateAsync(paper, nameof(paper.Title));

                }
                else if (orderDetail.ProductType == DgAyProductType.ViewALevelSchoolPermission)
                {
                    //解锁A级学校查阅权限
                    var permission = SchoolViewPermission.NewDraft(notification.Order.UserId.Value, orderDetail.ProductId);
                    await _schoolViewPermissionRepository.AddAsync(permission);
                }
            }
        }

    }
}
