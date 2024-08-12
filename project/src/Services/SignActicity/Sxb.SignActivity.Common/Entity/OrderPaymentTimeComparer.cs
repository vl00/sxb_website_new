using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.Entity
{
    public class OrderPaymentTimeComparer : IEqualityComparer<Order>
    {
        public bool Equals(Order x, Order y)
        {
            return x.Userid == y.Userid && x.Paymenttime == y.Paymenttime;
        }

        public int GetHashCode([DisallowNull] Order obj)
        {
            //return obj.Userid.GetHashCode() + obj.Paymenttime.GetHashCode();
            return obj.Id.GetHashCode();
        }
    }
}
