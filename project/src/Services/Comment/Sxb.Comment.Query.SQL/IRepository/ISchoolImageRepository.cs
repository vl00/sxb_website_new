using Sxb.Comment.Common.Entity;
using Sxb.Comment.Common.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.Comment.Query.SQL.IRepository
{
    public interface ISchoolImageRepository
    {
        Task<IEnumerable<SchoolImageInfo>> GetImageByDataSourceIDs(IEnumerable<Guid> ids, ImageType imageType);
    }
}
