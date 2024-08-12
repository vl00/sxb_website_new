using Sxb.Domain;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Domain.Events
{
    public class DgAyOrderPaySuccessDomainEvent : IDomainEvent
    {
        public  DgAyOrder Order { get; set; }

    }
}
