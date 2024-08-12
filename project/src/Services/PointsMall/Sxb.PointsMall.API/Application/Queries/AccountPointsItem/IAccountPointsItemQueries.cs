using Sxb.PointsMall.API.Application.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Queries.AccountPointsItem
{
    public interface IAccountPointsItemQueries
    {
        Task<AccountPointsDetails> GetAccountPointsDetails(GetAccountPointsDetailsFilter filter);
    }
}
