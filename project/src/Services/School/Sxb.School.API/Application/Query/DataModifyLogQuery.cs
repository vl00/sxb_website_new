using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class DataMOdifyLogQuery : IDataModifyLogQuery
    {
        readonly IDataModifyLogRepository _dataModifyLogRepository;

        public DataMOdifyLogQuery()
        {
        }

        public DataMOdifyLogQuery(IDataModifyLogRepository dataModifyLogRepository)
        {
            _dataModifyLogRepository = dataModifyLogRepository;
        }
        public async Task<IEnumerable<WechatModifyLogInfo>> ListByEIDAsync(IEnumerable<Guid> eids, DateTime createDate = default)
        {
            if (createDate == default) createDate = DateTime.Now.Date;
            return await _dataModifyLogRepository.ListByEIDAsync(eids, createDate);
        }

        public async Task<bool> SaveAsync(WechatModifyLogInfo entity)
        {
            if (entity == default) return false;
            if (entity.ID == default) return await _dataModifyLogRepository.InsertAsync(entity) > 0;
            else return await _dataModifyLogRepository.UpdateAsync(entity) >0;
        }
    }
}