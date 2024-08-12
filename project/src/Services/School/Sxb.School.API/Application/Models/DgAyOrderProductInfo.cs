using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using System;

namespace Sxb.School.API.Application.Models
{
    public class DgAyOrderProductInfo
    {

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OriginUnitPrice { get; set; }

        public DgAyProductType productType { get; set; }


        public int Number { get; set; }
    }
}
