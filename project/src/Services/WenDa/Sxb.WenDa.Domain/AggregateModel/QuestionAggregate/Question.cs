using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Domain.AggregateModel.QuestionAggregate
{
    public class Question : Entity<Guid>, IAggregateRoot
    {
        public int CollectCount { get; set; }
        public int LikeTotalCount { get; set; }

        public DateTime? ModifyDateTime { get; set; }

        public void CalcCollectCount(int value)
        {
            CollectCount += value;
            ModifyDateTime = DateTime.Now;
        }

        public void CalcLikeTotalCount(int value)
        {
            LikeTotalCount += value;
            ModifyDateTime = DateTime.Now;
        }
    }
}
