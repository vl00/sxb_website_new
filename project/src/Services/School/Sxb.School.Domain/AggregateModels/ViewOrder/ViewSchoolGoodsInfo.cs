using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.School.Domain.AggregateModels.ViewOrder
{
    public class ViewSchoolGoodsInfo:Entity<Guid>
    {
        public decimal Price { get; private set; }


        public ViewSchoolGoodsInfo(Guid id,decimal price)
        {
            this.Id = id;
            this.Price = price;
        }


    }
}
