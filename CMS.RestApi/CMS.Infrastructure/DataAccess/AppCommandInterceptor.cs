using LinqToDB.Common;
using LinqToDB.Interceptors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess
{
    public class AppCommandInterceptor : CommandInterceptor
    {
        private const string TransactionIsolationLevl = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
        public AppCommandInterceptor() { }

        public override Option<DbDataReader> ExecuteReader(CommandEventData eventData, DbCommand command, CommandBehavior commandBehavior, Option<DbDataReader> result)
        {
            Debug.WriteLine("AppComandIterceptor Executereader Called");
            SetTransactionLevel(command);
            EnsureConnection(command);
            return base.ExecuteReader(eventData, command, commandBehavior, result);        
        }
        public override Task<Option<DbDataReader>> ExecuteReaderAsync(CommandEventData eventData, DbCommand command, CommandBehavior commandBehavior, Option<DbDataReader> result,CancellationToken cancellationToken)
        {
            Debug.WriteLine("AppComandIterceptor Executereaderasync Called");
            SetTransactionLevel(command);
            EnsureConnection(command);
            return base.ExecuteReaderAsync(eventData, command, commandBehavior, result, cancellationToken);
        }
        public override Option<int> ExecuteNonQuery(CommandEventData eventData, DbCommand command, Option<int> result)
        {
            Debug.WriteLine("AppComandIterceptor ExecuteNonQuery Called");
            SetTransactionLevel(command);
            EnsureConnection(command);
            return base.ExecuteNonQuery(eventData, command,  result);

        }

        public override Task<Option<int>> ExecuteNonQueryAsync(CommandEventData eventData, DbCommand command, Option<int> result, CancellationToken cancellationToken)
        {
            Debug.WriteLine("AppComandIterceptor ExecuteNonQueryAsync Called");
            SetTransactionLevel(command);
            EnsureConnection(command);
            return base.ExecuteNonQueryAsync(eventData, command, result, cancellationToken);

        }

        public override Option<object> ExecuteScalar(CommandEventData eventData, DbCommand command,Option<object> result)
        {
            Debug.WriteLine("AppComandIterceptor ExecuteScalar Called");
            SetTransactionLevel(command);
            EnsureConnection(command);
            return base.ExecuteScalar(eventData, command, result);

        }
        public override Task<Option<object>> ExecuteScalarAsync(CommandEventData eventData, DbCommand command, Option<object> result, CancellationToken cancellationToken)
        {
            Debug.WriteLine("AppComandIterceptor ExecuteScalar Called");
            SetTransactionLevel(command);
            EnsureConnection(command);
            return base.ExecuteScalarAsync(eventData, command, result, cancellationToken);

        }

        private void SetTransactionLevel(IDbCommand command) 
        {
            SetTransaction(command);
        }


        private void EnsureConnection(IDbCommand command)
        {
            var connection = command.Connection;
            var timeOut = DateTime.Now.AddSeconds(connection.ConnectionTimeout);
            var isTimeOut = false;

            while (connection.State == ConnectionState.Connecting) 
            {
                if (DateTime.Now.CompareTo(timeOut) > 0) { 
                    isTimeOut = true;
                    break;                
                }
            }
            if (isTimeOut) {
                throw new TimeoutException("Connection to database time out.");            
            }
            if (connection.State != ConnectionState.Open)
            {
                Debug.WriteLine($"DataContext connection ReOpended");
            }
        
        }

        private void SetTransaction(IDbCommand command)
        {
            if (command.Transaction == null)
            {

                var transaction = GetTransaction(command.Connection);
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }
            }
        }

        private static DbTransaction GetTransaction(IDbConnection connection) 
        {
            object internalConn;
            var wrappConnectionProperty = connection.GetType().GetProperty("WrappedConnection");
            var innerConnectionProperty = connection.GetType().GetProperty("InnerConnection", BindingFlags.NonPublic | BindingFlags.Instance);
            object wrappedCon = null;
            if (wrappConnectionProperty != null) 
            {
                wrappedCon = wrappConnectionProperty.GetValue(connection);
            }

            if (wrappedCon != null) 
            {
                if (innerConnectionProperty != null)
                {
                    internalConn = innerConnectionProperty.GetValue(wrappedCon, null);
                }
                else 
                {
                    internalConn = wrappedCon;
                }            
            
            }
            else
            {


                if (innerConnectionProperty != null)
                {

                    internalConn = innerConnectionProperty.GetValue(connection, null);
                }
                else { 
                    internalConn = connection;
                }
            }

            var currentTransactionProperty = internalConn.GetType().GetProperty("CurrentTransaction",BindingFlags.NonPublic | BindingFlags.Instance);
            var currentTransaction = currentTransactionProperty?.GetValue(internalConn,null);
            if (currentTransaction != null) { 
                var realTransactionProperty = currentTransaction.GetType().GetProperty("Parent",BindingFlags.NonPublic | BindingFlags.Instance);
                var realTransaction = realTransactionProperty?.GetValue(currentTransaction, null);
                return (DbTransaction)realTransaction;
            
            }

            return null;
        }

    }
}
