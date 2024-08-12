using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.Foundation;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Query.SQL.DB;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.Repository
{
    public class CorrectionMsgRepository : ICorrectionMsgRepository
    {
        readonly SchoolDataDB _schoolDataDB;
        public CorrectionMsgRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<int> InsertAsync(CorrectionMessageInfo entity)
        {
            if (entity == null || entity.IdentityType == IdentityType.Unknow || string.IsNullOrWhiteSpace(entity.Mobile)
                || string.IsNullOrWhiteSpace(entity.Content) || string.IsNullOrWhiteSpace(entity.Nickname)) return 0;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            if (entity.CreateTime == default) entity.CreateTime = entity.CreateTime.CnNow();
            return await _schoolDataDB.Connection.CommandSet<CorrectionMessageInfo>().InsertAsync(entity);
        }
    }
}