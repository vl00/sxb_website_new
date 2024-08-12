using System.Data;

namespace Sxb.Comment.Common.DB
{
    public class SchoolProductDB
    {
        public IDbConnection Connection { get; set; }
        public IDbConnection SlaveConnection { get; set; }
    }
}