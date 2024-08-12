using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.User.API.RequestContract.Collect
{
    public class CheckIsCollectRequest
    {
        public Guid DataID { get; set; }
        public Guid UserID { get; set; }
    }
}
