using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface IDataModifyLogRepository
    {
        /// <summary>
        /// 根据修改记录
        /// </summary>
        /// <param name="eid">学部记录</param>
        /// <param name="createDate">修改时间</param>
        /// <returns></returns>
        Task<IEnumerable<WechatModifyLogInfo>> ListByEIDAsync(IEnumerable<Guid> eids, DateTime createDate);
        Task<int> UpdateAsync(WechatModifyLogInfo entity);
        Task<int> InsertAsync(WechatModifyLogInfo entity);
    }
}
