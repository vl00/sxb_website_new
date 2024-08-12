using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Sxb.Recommend.Infrastructure.Repository.SQLServer.DataBases
{
   public abstract class DataBase
    {
        public IDbConnection Connection { get; set; }
    }
}
