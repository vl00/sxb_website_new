using Microsoft.Data.SqlClient;
using Sxb.PointsMall.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Infrastructure
{
    public class PointsMallDbContext : IUnitOfWork
    {


        private IDbConnection _dbConnection;
        private IDbTransaction _currentTransaction;
        /// <summary>
        /// 可读可写连接，复杂查询尽量不要使用该连接。
        /// </summary>
        public IDbConnection Connection => _dbConnection;

        public IDbTransaction CurrentTransaction => _currentTransaction;



        public static PointsMallDbContext CreateSqlConnectionFrom(string conStr)
        {
            SqlConnection connection = new SqlConnection(conStr);
            return new PointsMallDbContext(connection);
        }

        public PointsMallDbContext(IDbConnection dbConnection)
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
