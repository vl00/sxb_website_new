using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Model
{
    public interface ISettlementReposiroty
    {

        Task AddAsync(Settlement settlement);

        Task<bool> UpdateStatusByAsync(string orderNum,SettlementStatus settlementStatus,string remark);

        Task<Settlement> FindByOrderNumAsync(string orderNum);
    }
}
