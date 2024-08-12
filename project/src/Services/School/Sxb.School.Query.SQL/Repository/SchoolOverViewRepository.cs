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
    public class SchoolOverViewRepository : ISchoolOverViewRepository
    {
        readonly SchoolDataDB _schoolDataDB;

        public SchoolOverViewRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<bool> DeleteIFExist(Guid eid)
        {
            return await _schoolDataDB.SlaveConnection.CommandSet<SchoolOverViewInfo>().Where(p => p.EID == eid).DeleteAsync() > 0;
        }

        public async Task<SchoolOverViewInfo> GetByEID(Guid eid)
        {
            if (eid == default) return null;
            var find = await _schoolDataDB.SlaveConnection.QuerySet<SchoolOverViewInfo>().Where(p => p.EID == eid).GetAsync();
            if (find?.ID != default) return find;
            return default;
        }

        public async Task<bool> InsertAsync(SchoolOverViewInfo entity)
        {
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return (await _schoolDataDB.Connection.CommandSet<SchoolOverViewInfo>().InsertAsync(entity)) > 0;
        }

        public async Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids)
        {
            var effectCount = 0;
            if (eids?.Any() == true)
            {
                var index = 0;
                IEnumerable<Guid> ids;
                while ((ids = eids.Skip(index).Take(200))?.Any() == true)
                {
                    effectCount += await _schoolDataDB.Connection.CommandSet<SchoolOverViewInfo>().Where($"EID IN{ids.ToSQLInString()}").DeleteAsync();
                    index += 200;
                }
            }
            return effectCount;
        }

        public async Task<int> UpdateAsync(SchoolOverViewInfo entity)
        {
            if (entity == default || entity.ID == default) return 0;
            return await _schoolDataDB.Connection.CommandSet<SchoolOverViewInfo>().UpdateAsync(entity);
        }
    }
}
