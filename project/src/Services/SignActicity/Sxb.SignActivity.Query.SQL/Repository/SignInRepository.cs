﻿using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.MsSql;
using Sxb.SignActivity.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sxb.SignActivity.Common.Entity;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Query.SQL.Repository
{
    public class SignInRepository : ISignInRepository
    {
        private readonly OrgDB _orgDB;

        public SignInRepository(OrgDB orgDB)
        {
            _orgDB = orgDB ?? throw new ArgumentNullException(nameof(orgDB));
        }

        public async Task<IEnumerable<SignIn>> GetSignInsAsync()
        {
            return await _orgDB.SlaveConnection.QuerySet<SignIn>().ToIEnumerableAsync();
        }


        public async Task<IEnumerable<SignIn>> GetSignInsAsync(string buNo)
        {
            return await _orgDB.SlaveConnection.QuerySet<SignIn>()
                .Where(s => s.BuNo == buNo && s.IsValid == true)
                .ToIEnumerableAsync();
        }
    }
}
