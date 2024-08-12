using System.Data;

namespace Sxb.ArticleMajor.Query.SQL
{
    public class ArticleMajorDB
    {
        public IDbConnection Connection { get; set; }
        public IDbConnection SlaveConnection { get; set; }
    }
}
