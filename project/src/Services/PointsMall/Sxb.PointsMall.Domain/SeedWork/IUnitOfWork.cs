using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Sxb.PointsMall.Domain.SeedWork
{
    public interface IUnitOfWork
    {

        IDbTransaction BeginTransaction();
        void CommitTransaction();
        void RollBackTransaction();
    }
}
