using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Domain.AggregateModels.DgAyOrderAggregate
{
    public class DgAyOrderDetail : Entity<Guid>, IAggregateRoot
    {

        public Guid OrderId { get; private set; }

        public Guid ProductId { get; private set; }

        public DgAyProductType ProductType { get; private set; }

        public string ProductName { get; private set; }

        public string ProductDesc { get; private set; }

        public int Number { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal OriginUnitPrice { get; private set; }

        public DateTime CreateTime { get; private set; }

        public void SetOrderId(Guid orderId)
        {
            this.OrderId = orderId;
        }
        public static DgAyOrderDetail NewDraft(Guid productId, DgAyProductType productType, string productName, string productDesc, int number, decimal unitPrice, decimal originUnitPrice)
        {
            var detail =  new DgAyOrderDetail();
            detail.Id = Guid.NewGuid();
            detail.ProductId = productId;
            detail.ProductType = productType;
            detail.ProductName = productName;
            detail.ProductDesc= productDesc;
            detail.Number= number;
            detail.UnitPrice = unitPrice;
            detail.OriginUnitPrice = originUnitPrice;
            detail.CreateTime = DateTime.Now;
            return detail;
        
        }

    

    }
}
