using Sxb.Domain;
using Sxb.Recommend.Application.Services;
using Sxb.Recommend.Domain.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.DomainEventHandler
{
    public class MapFeatureChangeEventHandler : IDomainEventHandler<MapFeatureChangeEvent>
    {
        ISchoolMapService _schoolMapService;
        IArticleMapService _articleMapService;
        public MapFeatureChangeEventHandler(ISchoolMapService schoolMapService
            , IArticleMapService articleMapService)
        {
            _schoolMapService = schoolMapService;
            _articleMapService = articleMapService;
        }
        public async Task Handle(MapFeatureChangeEvent notification, CancellationToken cancellationToken)
        {
            if (notification.MapFeature.Type == 1)
            {
                await _schoolMapService.ClearAll();
            }
            if (notification.MapFeature.Type == 2)
            {
                await _articleMapService.ClearAll();
            }


        }
    }
}
