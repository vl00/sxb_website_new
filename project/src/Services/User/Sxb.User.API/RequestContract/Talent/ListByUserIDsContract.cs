using System;
using System.Collections.Generic;

namespace Sxb.User.API.RequestContract.Talent
{
    public class ListByUserIDsRequest
    {
        public IEnumerable<Guid> UserIDs { get; set; }
    }
}
