using Sxb.Domain;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Event
{
    /// <summary>
    /// 学校跳转频率发生变更事件
    /// </summary>
    public class SchoolOpenTimeIsChangeEvent:IDomainEvent
    {
        public SchoolRedirectFrequency SchoolRedirectFrequency { get;private set; }
        public SchoolOpenTimeIsChangeEvent(SchoolRedirectFrequency schoolRedirectFrequency)
        {
            SchoolRedirectFrequency = schoolRedirectFrequency;
        }
    }
}
