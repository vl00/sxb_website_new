using Sxb.School.Common.Entity;
using Sxb.School.Common.DTO;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sxb.School.API.Application.Query
{
    public interface ICallSchoolApiQuery
    {
        /// <summary>根据学部eid查询学校名,id,no</summary>
        Task<List<SchoolIdAndNameDto>> GetSchoolsIdAndName(string[] eids);

    }
}