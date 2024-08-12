using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sxb.Framework.Foundation;
using Microsoft.Data.SqlClient;

namespace Sxb.SignActivity.Infrastructure
{
    internal static class DbHelper
    {
        public static IEnumerable<TEntity> Query<TEntity>(this DbContext context, string sql, params SqlParameter[] paras) 
            where TEntity : new()
        {
            DbConnection dbConnection = context.Database.GetDbConnection();
            DbCommand dbCommand = dbConnection.CreateCommand();
            //context.Database.OpenConnection();

            if (paras != null)
            {
                dbCommand.Parameters.AddRange(paras);
            }
            dbCommand.CommandText = sql;
            DataTable dataTable = new DataTable();
            using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
            {
                dataTable.Load(dbDataReader);
                dbDataReader.Close();
                dbCommand.Parameters.Clear();
            }
            //context.Database.CloseConnection();
            List<TEntity> rez = dataTable.ToList<TEntity>();
            return rez ?? new List<TEntity>();
        }

        public static TEntity QueryFirstOrDefault<TEntity>(this DbContext context, string sql, params SqlParameter[] paras)
            where TEntity : new()
        {
            return context.Query<TEntity>(sql, paras).FirstOrDefault();
        }

        public static TEntity QueryFirstOrDefault<TEntity>(this DbContext context, string sql, object paras = null)
            where TEntity : new()
        {
            return context.Query<TEntity>(sql, GetSqlParameter(paras)).FirstOrDefault();
        }

        public static SqlParameter[] GetSqlParameter(object paras = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            if (paras != null)
            {
                var type = paras.GetType();
                var props = type.GetProperties();
                foreach (var prop in props)
                {
                    if (prop.CanRead)
                    {
                        var p = new SqlParameter(prop.Name, prop.GetValue(paras, null));
                        sqlParameters.Add(p);
                    }
                }
            }
            return sqlParameters.ToArray(); 
        }
    }
}
