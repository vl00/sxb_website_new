using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Model
{
    public class User
    {
        public Guid Id { get; init; }

        public string NickName { get; init; }

        public string NationCode { get; init; } 

        public string Mobile { get; init; }

    }
}
