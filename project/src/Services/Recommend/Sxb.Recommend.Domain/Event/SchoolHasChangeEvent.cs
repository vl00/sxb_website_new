using Sxb.Domain;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Event
{

    /// <summary>
    /// 学校发生变更事件
    /// </summary>
    public class SchoolHasChangeEvent : IDomainEvent
    {
        public School School { get;private set; }

        /// <summary>
        /// 变更类型 1->新增 2->删除 3->修改
        /// </summary>
        public EntityChangeType ChangeType { get;private set; }
        public SchoolHasChangeEvent(School school, EntityChangeType changeType)
        {
            this.School = school;
            this.ChangeType = changeType;
        }

    }
}
