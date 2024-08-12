using MediatR;
using Sxb.School.Domain.AggregateModels.ViewPermission;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Commands
{
    public class OpenViewPermissionCommandHandler : IRequestHandler<OpenViewPermissionCommand, bool>
    {
        ISchoolViewPermissionRepository _schoolViewPermissionRepository;

        public OpenViewPermissionCommandHandler(ISchoolViewPermissionRepository schoolViewPermissionRepository)
        {
            _schoolViewPermissionRepository = schoolViewPermissionRepository;
        }
        public async Task<bool> Handle(OpenViewPermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = await _schoolViewPermissionRepository.FindFromUserAndExtAsync(request.UserId, request.ExtId);
            if (permission != null)
                if (permission.IsValid) throw new Exception("该查阅权限已开通。");
                else throw new Exception("该查阅权限已禁用。");
            permission = SchoolViewPermission.NewDraft(request.UserId, request.ExtId);
            await _schoolViewPermissionRepository.AddAsync(permission);
            return true;
        }
    }
}
