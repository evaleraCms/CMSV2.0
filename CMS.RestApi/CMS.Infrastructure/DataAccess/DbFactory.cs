using CMS.Infrastructure.DataAccess.DbInteractions.Contracts;
using CMS.Infrastructure.DataAccess.DbInteractions.Events;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess
{
    public class DbFactory : Disposable, IDbFactory
    {
        public void DisposeCurrentContext()
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string sqlStatement, params DataParameter[] dataParameters)
        {
            throw new NotImplementedException();
        }

        public IList<T> ExecuteReader<T>(string sqllStatement, Func<IDataReader, T> onRead, params DataParameter[] dataParameters)
        {
            throw new NotImplementedException();
        }

        public T ExecuteStoreProcedure<T>(string procedureName, params DataParameter[] parameteres)
        {
            throw new NotImplementedException();
        }

        public int ExecuteStoreProcedure(string procedureName, params DataParameter[] dataParameters)
        {
            throw new NotImplementedException();
        }

        public DataContext Get()
        {
            throw new NotImplementedException();
        }

        public Guid? GetCurrentContextId()
        {
            throw new NotImplementedException();
        }

        public void PublishChangedEvent<TEntity>(FieldChangedEventArgs<TEntity> eventArgs)
        {
            throw new NotImplementedException();
        }

        public void PublishChangingEvent<TEntity>(FieldChangingEventArgs<TEntity> eventArgs)
        {
            throw new NotImplementedException();
        }

        public void PublishDeletedEvent<TEntity>(FieldDeletedEventArgs<TEntity> eventArgs)
        {
            throw new NotImplementedException();
        }

        public void PublishDeletingEvent<TEntity>(FieldDeletingEventArgs<TEntity> eventArgs)
        {
            throw new NotImplementedException();
        }

        public void PublishInsertedEvent<TEntity>(FieldInsertedEventArgs<TEntity> eventArgs)
        {
            throw new NotImplementedException();
        }

        public void PublishInsertingEvent<TEntity>(FieldInsertingEventArgs<TEntity> eventArgs)
        {
            throw new NotImplementedException();
        }

        public IList<T> Query<T>(string sql, params DataParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<T> QueryProc<T>(string procedureName, params DataParameter[] parameteres)
        {
            throw new NotImplementedException();
        }

        public Task RunAsysnc(Action<IDbFactory> action)
        {
            throw new NotImplementedException();
        }

        public Guid SetNewContextId()
        {
            throw new NotImplementedException();
        }
    }
}
