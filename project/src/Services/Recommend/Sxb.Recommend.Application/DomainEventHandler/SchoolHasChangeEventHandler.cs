using Microsoft.Extensions.DependencyInjection;
using Sxb.Domain;
using Sxb.Recommend.Application.Services;
using Sxb.Recommend.Domain.Event;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.DomainEventHandler
{

    /// <summary>
    /// 处理学校发生变更事件
    /// </summary>
    public class SchoolHasChangeEventHandler : IDomainEventHandler<SchoolHasChangeEvent>
    {
        IServiceProvider _serviceProvider;
        public SchoolHasChangeEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(SchoolHasChangeEvent notification, CancellationToken cancellationToken)
        {
            if (notification.ChangeType == Domain.Enum.EntityChangeType.Modifiy
                ||
               notification.ChangeType == Domain.Enum.EntityChangeType.Add
                )
            {
                //_ = Task.Run(async () =>
                //{
                    //找出关联度最高并且与该学校有关联的学校去更新它们的SchoolMaps
                    //using (var scope = _serviceProvider.CreateScope())
                    //{
                    //    IRecommendService recommendService = scope.ServiceProvider.GetService<IRecommendService>();
                    //    ISchoolMapRepository schoolMapRepository = scope.ServiceProvider.GetService<ISchoolMapRepository>();
                    //    List<Guid> schoolIds = new List<Guid>();
                    //    //获取关联的SchoolMaps
                    //    var schoolMapsFromDB = await schoolMapRepository.GetSchoolMaps(notification.School);
                    //    //生成当前学校的SchoolMaps
                    //    var schoolMapsFromGenerate = await recommendService.UpsertSchoolMaps(notification.School);
                    //    schoolIds.AddRange(schoolMapsFromDB.Select(s => s.SIdP));
                    //    schoolIds.AddRange(schoolMapsFromGenerate.Select(s => s.SIdS));
                    //    if (schoolIds.Any())
                    //    {
                    //        await recommendService.UpsertSchoolMaps(schoolIds.Distinct());
                    //    }

                    //}
                //});

            }
            //return Task.CompletedTask;
        }
    }
}
