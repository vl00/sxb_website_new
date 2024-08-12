using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public interface ISchoolRedirectInsideService
    {
        Task Add(SchoolRedirectInside schoolRedirectInside);
        Task Add(string shortNo1, string shortNo2);
        Task<List<SchoolRedirectInside>> ListAsync(Guid primaryId);
    }
}
