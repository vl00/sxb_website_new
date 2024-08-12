using System.Data;

namespace Sxb.User.Query.SQL.DB
{
    public class SchoolUserDB
    {
        public IDbConnection Connection { get; set; }
        public IDbConnection SlaveConnection { get; set; }
    }
}
