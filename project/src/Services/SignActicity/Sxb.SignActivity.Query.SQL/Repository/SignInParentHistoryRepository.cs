using Kogel.Dapper.Extension.Extension;
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
    public class SignInParentHistoryRepository : ISignInParentHistoryRepository
    {
        private readonly OrgDB _orgDB;

        public SignInParentHistoryRepository(OrgDB orgDB)
        {
            _orgDB = orgDB ?? throw new ArgumentNullException(nameof(orgDB));
        }


        public async Task<IEnumerable<SignInParentHistory>> GetListAsync(string buNo)
        {
            return await _orgDB.SlaveConnection.QuerySet<SignInParentHistory>()
                .Where(s => s.BuNo == buNo && s.IsValid == true)
                .ToIEnumerableAsync();
        }

    }
}
