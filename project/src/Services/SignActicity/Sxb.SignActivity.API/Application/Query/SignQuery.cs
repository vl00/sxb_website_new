using Sxb.SignActivity.Common.Entity;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Sxb.SignActivity.Query.SQL.IRepository;

namespace Sxb.SignActivity.API.Application.Query
{
    public class SignQuery : ISignQuery
    {

        private readonly ISignInRepository _signRepository;

        public SignQuery(ISignInRepository signRepository)
        {
            _signRepository = signRepository ?? throw new ArgumentNullException(nameof(signRepository));
        }

        public async Task<IEnumerable<SignIn>> GetSignInsAsync()
        {
            return await _signRepository.GetSignInsAsync();
        }
    }
}
