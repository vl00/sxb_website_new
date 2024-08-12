using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Domain.Exceptions
{
    public class AccountPointsNotEnoughException: Exception
    {
        public AccountPointsNotEnoughException(string message) : base(message)
        {

        }
    }
}
