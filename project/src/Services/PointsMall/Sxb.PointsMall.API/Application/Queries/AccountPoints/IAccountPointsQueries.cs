using Sxb.PointsMall.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Queries.AccountPoints
{
    public interface IAccountPointsQueries
    {

        Task<AccountPoints> GetAccountPoints(Guid userId);

        Task<IEnumerable<Guid>> GetPointsOverZeroUserIds();

    }
}
