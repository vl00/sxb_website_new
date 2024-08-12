using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sxb.School.Common.Entity;

namespace Sxb.School.API.Application.Query
{
    public interface IDataModifyLogQuery
    {
        Task<IEnumerable<WechatModifyLogInfo>> ListByEIDAsync(IEnumerable<Guid> eids, DateTime createDate = default);

        Task<bool> SaveAsync(WechatModifyLogInfo entity);
    }
}