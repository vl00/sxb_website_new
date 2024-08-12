using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Model
{
    public interface IUserRepository
    {
        Task<User> FindAsync(Guid id);
    }
}
