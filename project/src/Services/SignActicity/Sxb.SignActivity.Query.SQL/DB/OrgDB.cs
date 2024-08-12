using System.Data;

namespace Sxb.SignActivity.Query.SQL
{
    public class OrgDB
    {
        public IDbConnection Connection { get; set; }
        public IDbConnection SlaveConnection { get; set; }
    }
}
