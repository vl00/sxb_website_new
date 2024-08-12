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
    public class SchoolProjectRepository : ISchoolProjectRepository
    {
        readonly SchoolDataDB _schoolDataDB;

        public SchoolProjectRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<IEnumerable<OnlineSchoolProjectInfo>> GetByEID(Guid eid)
        {
            if (eid == default) return null;
            return await _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolProjectInfo>().Where(p => p.EID == eid).ToIEnumerableAsync();
        }

        public async Task<int> InsertAsync(OnlineSchoolProjectInfo entity)
        {
            if (entity == null) return 0;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return await _schoolDataDB.Connection.CommandSet<OnlineSchoolProjectInfo>().InsertAsync(entity);
        }

        public async Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids)
        {
            var effectCount = 0;
            if (eids?.Any() == true)
            {
                var index = 0;
                IEnumerable<Guid> tmp_IDs;
                while ((tmp_IDs = eids.Skip(index).Take(200))?.Any() == true)
                {
                    effectCount += await _schoolDataDB.Connection.CommandSet<OnlineSchoolProjectInfo>().Where($"EID in {tmp_IDs.ToSQLInString()}").DeleteAsync();
                    index += 200;
                }
            }
            return effectCount;
        }

        public async Task<int> UpdateAsync(OnlineSchoolProjectInfo entity)
        {
            if (entity == default || entity.ID == default) return 0;
            return await _schoolDataDB.Connection.CommandSet<OnlineSchoolProjectInfo>().UpdateAsync(entity);
        }
    }
}
