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
    public class SignConfigRepository : ISignConfigRepository
    {
        private readonly OrgDB _orgDB;

        public SignConfigRepository(OrgDB orgDB)
        {
            _orgDB = orgDB ?? throw new ArgumentNullException(nameof(orgDB));
        }

        public async Task<SignConfig> GetAsync(string buNo)
        {
            return (await _orgDB.Connection.QuerySet<SignConfig>().Where(s => s.BuNo == buNo && s.Isvalid == true).ToIEnumerableAsync()).FirstOrDefault();
        }
    }
}
