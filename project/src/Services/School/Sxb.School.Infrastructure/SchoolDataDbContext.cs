
using Sxb.School.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.Infrastructure
{
    public class SchoolDataDbContext : IUnitOfWork
    {


        private IDbConnection _dbConnection;
        private IDbTransaction _currentTransaction;


        public IDbConnection Connection => _dbConnection;

        public IDbTransaction CurrentTransaction => _currentTransaction;


        public SchoolDataDbContext(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IDbTransaction BeginTransaction()
        {
            if (_currentTransaction != null) return _currentTransaction;
            if (_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();
            _currentTransaction = _dbConnection.BeginTransaction();
            return _currentTransaction;
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
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }

            if (_dbConnection != null)
            {
                _dbConnection.Dispose();
                _dbConnection = null;
            }

        }


    }
}
