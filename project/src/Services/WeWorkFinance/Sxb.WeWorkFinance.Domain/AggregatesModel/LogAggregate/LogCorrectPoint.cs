using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Domain.AggregatesModel.LogAggregate
{
    public class LogCorrectPoint : Entity<int>, IAggregateRoot
    {
        public string InviterUnionId { get; set; }
        public Guid UserId { get; set; }
        public int OriginPoint { get; set; }
        public int TotalPoint { get; set; }
        public int AddPoint { get; set; }
        public int InvalidPoint { get; set; }
        public int UsePoint { get; set; }
        public int InvalidUsePoint { get; set; }
        public int InvalidOrderCount { get; set; }
        public int ReturnPoint { get; set; }
        public int FinalPoint { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


        public void Add(string inviterUnionId, Guid userId, int originPoint, int totalPoint, int addPoint, int invalidPoint, int usePoint, int invalidUsePoint, int invalidOrderCount, int returnPoint, int finalPoint, DateTime startTime, DateTime endTime)
        {
            InviterUnionId = inviterUnionId;
            UserId = userId;
            OriginPoint = originPoint;
            TotalPoint = totalPoint;
            AddPoint = addPoint;
            InvalidPoint = invalidPoint;
            UsePoint = usePoint;
            InvalidUsePoint = invalidUsePoint;
            InvalidOrderCount = invalidOrderCount;
            ReturnPoint = returnPoint;
            FinalPoint = finalPoint;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
