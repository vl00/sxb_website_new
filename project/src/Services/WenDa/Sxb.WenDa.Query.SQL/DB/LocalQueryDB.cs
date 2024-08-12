using Dapper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System.Data;

namespace Sxb.WenDa.Query.SQL
{
    public class LocalQueryDB
    {
        public IDbConnection Connection { get; set; }
        public IDbConnection SlaveConnection { get; set; }


        public async Task<Page<T>> QueryPageAsync<T>(string sql, string sqlTotal, object param)
        {
            var sqlAll = $"{sqlTotal};{sql};";

            var gridReader = await SlaveConnection.QueryMultipleAsync(sqlAll, param);
            var total = await gridReader.ReadFirstAsync<int>();
            var data = await gridReader.ReadAsync<T>();
            return data.ToPage(total);
        }
    }
}
