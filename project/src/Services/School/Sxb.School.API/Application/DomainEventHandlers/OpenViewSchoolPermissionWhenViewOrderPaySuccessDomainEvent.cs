using Sxb.Domain;
using Sxb.School.Domain.AggregateModels.ViewPermission;
using Sxb.School.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.DomainEventHandlers
{
    public class OpenViewSchoolPermissionWhenViewOrderPaySuccessDomainEvent : IDomainEventHandler<ViewOrderPaySuccessDomainEvent>
    {
        ISchoolViewPermissionRepository _schoolViewPermissionRepository;

        public OpenViewSchoolPermissionWhenViewOrderPaySuccessDomainEvent(ISchoolViewPermissionRepository schoolViewPermissionRepository)
        {
            _schoolViewPermissionRepository = schoolViewPermissionRepository;
        }

        public async Task Handle(ViewOrderPaySuccessDomainEvent notification, CancellationToken cancellationToken)
        {
            var permission = SchoolViewPermission.NewDraft(notification.UserId, notification.ViewSchoolGoodsInfo.Id);
            await _schoolViewPermissionRepository.AddAsync(permission);
        }
    }
}
