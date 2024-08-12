using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.Foundation;
using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.DB;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.Repository
{
    public class DataModifyLogRepository : IDataModifyLogRepository
    {
        readonly SchoolDataDB _schoolDataDB;

        public DataModifyLogRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<int> InsertAsync(WechatModifyLogInfo entity)
        {
            if (entity == default || entity.EID == default || entity.CreateDate == default) return 0;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return await _schoolDataDB.Connection.CommandSet<WechatModifyLogInfo>().InsertAsync(entity);
        }

        public async Task<IEnumerable<WechatModifyLogInfo>> ListByEIDAsync(IEnumerable<Guid> eids, DateTime createDate)
        {
            if (eids == default || !eids.Any() || createDate == default) return default;
            return await _schoolDataDB.SlaveConnection.QuerySet<WechatModifyLogInfo>().Where(p => p.CreateDate == createDate).Where($"EID IN {eids.ToSQLInString()}").ToIEnumerableAsync();
        }

        public async Task<int> UpdateAsync(WechatModifyLogInfo entity)
        {
            if (entity == default || entity.EID == default || entity.CreateDate == default) return 0;
            return await _schoolDataDB.Connection.CommandSet<WechatModifyLogInfo>().UpdateAsync(entity);
        }
    }
}
