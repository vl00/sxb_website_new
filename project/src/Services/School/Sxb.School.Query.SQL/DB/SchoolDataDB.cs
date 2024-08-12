using System.Data;

namespace Sxb.School.Query.SQL.DB
{
    public class SchoolDataDB
    {
        public IDbConnection Connection { get; set; }
        public IDbConnection SlaveConnection { get; set; }
    }
}
