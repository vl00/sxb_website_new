using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using Sxb.School.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Domain.AggregateModels.DgAyUserQaPaperAggregate
{
    public interface IDgAyUserQaPaperRepository : IRepository<DgAyUserQaPaper>
    {
        Task<DgAyUserQaPaper> GetAsync(Guid id);

        /// <summary>
        /// 获取Paper在当日已经解锁列表中的排序号，如果paper的排序号为空，则返回Null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int?> GetTimesAsync(Guid id);

        Task<bool> UpdateAsync(DgAyUserQaPaper qaPaper, params string[] fields);
    }
}
