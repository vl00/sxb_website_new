using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface ICorrectionMsgQuery
    {
        Task<bool> InsertAsync(CorrectionMessageInfo entity);
    }
}
