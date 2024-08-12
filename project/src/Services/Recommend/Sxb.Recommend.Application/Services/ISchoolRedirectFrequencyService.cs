using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public interface ISchoolRedirectFrequencyService
    {

        Task<SchoolRedirectFrequency> QueryFrequencyAsync(Guid sidp, Guid sids);
        Task FlushFrequency();

        Task FlushFrequencyToLocal();

        /// <summary>
        /// 从本地缓存读取
        /// </summary>
        /// <returns></returns>
        IEnumerable<SchoolRedirectFrequency> ReadFrequencies();

        Task<SchoolRedirectFrequency> GetFrequency(Guid sidp, Guid sids);

        Task<IEnumerable<SchoolRedirectFrequency>> GetFrequenciesAsync(Guid sidp);
    }
}
