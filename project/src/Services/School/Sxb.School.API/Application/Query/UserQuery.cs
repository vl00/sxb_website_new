using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Common.OtherAPIClient.User;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class UserQuery : IUserQuery
    {
        readonly IUserAPIClient _userAPIClient;

        public UserQuery(IUserAPIClient userAPIClient)
        {
            _userAPIClient = userAPIClient;
        }

        public async Task<bool> CheckIsSubscribe(string groupCode, Guid subjectId, Guid userId)
        {
            return await _userAPIClient.CheckIsSubscribe(groupCode, subjectId, userId);
        }
    }
}
