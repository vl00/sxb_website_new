using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using System;
using System.Collections.Generic;

namespace Sxb.School.API.Application.Queries.DgAyOrder
{
    public class DgAyOrderStateSummary
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DgAyOrderState State { get; set; }
    }

    public class OrderDetail {
        public Guid Id { get; set; }
        public string Num { get; set; }
        public Guid? UserId { get; set; }

        public DgAyOrderPayWay? PayWay { get; set; }

        public decimal Amount { get; set; }

        public decimal? Payment { get; set; }

        public DgAyOrderState Status { get; set; }
        public string Remark { get; set; }


        public DateTime ExpireTime { get; set; }

        public List<ProductInfo> ProductInfos { get; set; }
    }


    public class ProductInfo {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DgAyProductType Type { get; set; }

        public string Desc { get; set; }
        public int Number { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal OriginUnitPrice { get; set; }
    }
}
