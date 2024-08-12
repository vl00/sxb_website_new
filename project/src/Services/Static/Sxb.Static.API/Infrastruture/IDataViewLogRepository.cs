using Sxb.Static.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Static.API.Infrastruture
{
    public interface IDataViewLogRepository
    {

        Task AddAsync(DataViewLog dataViewLog);

        Task AddsAsync(IEnumerable<DataViewLog> dataViewLogs);

    }
}
