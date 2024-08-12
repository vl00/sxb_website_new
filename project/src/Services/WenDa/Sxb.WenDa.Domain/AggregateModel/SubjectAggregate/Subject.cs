using Sxb.Domain;
using Sxb.WenDa.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Domain.AggregateModel.SubjectAggregate
{
    public class Subject : Entity<Guid>, IAggregateRoot
    {
        public int CollectCount { get; set; }

        public DateTime? ModifyDateTime { get; set; }

        public void CalcCollectCount(int value)
        {
            CollectCount += value;
            ModifyDateTime = DateTime.Now;
        }
    }
}
