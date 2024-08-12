using Kogel.Dapper.Extension.MsSql;
using Sxb.Comment.Common.DB;
using Sxb.Comment.Common.Entity;
using Sxb.Comment.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Comment.Query.SQL.IRepository
{
    public class SchoolImageRepository : ISchoolImageRepository
    {
        readonly SchoolProductDB _schoolProductDB;

        public SchoolImageRepository(SchoolProductDB schoolProductDB)
        {
            _schoolProductDB = schoolProductDB;
        }

        public async Task<IEnumerable<SchoolImageInfo>> GetImageByDataSourceIDs(IEnumerable<Guid> ids, ImageType imageType)
        {
            if (ids == null || !ids.Any()) return null;
            var query = _schoolProductDB.SlaveConnection.QuerySet<SchoolImageInfo>().Where($"DataSourcetID in ('{string.Join(',', ids)}')");
            if (imageType > ImageType.Unknow) query = query.Where(p => p.ImageType == imageType);
            return await query.ToIEnumerableAsync();
        }
    }
}
