using System;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface IUserQuery
    {
        Task<bool> CheckIsSubscribe(string groupCode, Guid subjectId, Guid userId);
    }
}