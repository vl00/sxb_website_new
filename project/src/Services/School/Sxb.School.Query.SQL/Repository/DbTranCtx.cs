using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.DB
{
    /// <summary>
    /// 内部使用
    /// </summary>
    internal class DbTranCtx : IDisposable
    {
        private IDbConnection _dbConnection;
        private IDbTransaction _currentTransaction;
        private bool isOpened;

        public IDbConnection Connection => _dbConnection;
        public IDbTransaction CurrentTransaction => _currentTransaction;

        /// <summary>
        /// 内部使用
        /// </summary>
        public DbTranCtx(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void BeginTransaction()
        {
            if (_currentTransaction != null) return;
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
                isOpened = true;
            }
            _currentTransaction = _dbConnection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_currentTransaction == null) return;
            try
            {
                _currentTransaction.Commit();
            }
            catch
            {
                RollBackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollBackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            catch { }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void Dispose()
        {
            if (_currentTransaction != null)
            {
                RollBackTransaction();
            }
            if (_dbConnection != null)
            {
                if (isOpened)
                {
                    try { _dbConnection.Close(); } catch { }
                    isOpened = false;
                }
                _dbConnection = null;
            }
        }


    }
}
