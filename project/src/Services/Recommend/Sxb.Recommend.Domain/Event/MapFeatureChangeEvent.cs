using Sxb.Domain;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Event
{

    /// <summary>
    /// 特征值变更事件
    /// </summary>
    public class MapFeatureChangeEvent : IDomainEvent
    {
        public MapFeature  MapFeature { get; private set; }
        public MapFeatureChangeEvent(MapFeature  mapFeature)
        {
            MapFeature = mapFeature;
        }
    }
}
