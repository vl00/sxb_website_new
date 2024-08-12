using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Model
{
    public interface IIDCardRepository
    {

        Task<IDCard> FindByUserIdAsync(Guid userId, string number);

        Task<bool> AddAsync(IDCard IDCard);

        Task<bool> UpdateAsync(IDCard IDCard);
    }
}
