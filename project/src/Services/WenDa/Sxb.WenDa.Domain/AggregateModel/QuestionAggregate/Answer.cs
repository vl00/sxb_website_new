using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Domain.AggregateModel.QuestionAggregate
{
    public class Answer : Entity<Guid>, IAggregateRoot
    {
        public int LikeCount { get; set; }

        public DateTime? ModifyDateTime { get; set; }
        public Guid Qid { get; set; }

        public void CalcLikeCount(int value)
        {
            LikeCount += value;
            ModifyDateTime = DateTime.Now;
        }
    }
}
