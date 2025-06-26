using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LinqToDB.Data;
using System.Data;
using CMS.Infrastructure.DataAccess.DbInteractions.Events;

namespace CMS.Infrastructure.DataAccess.DbInteractions.Contracts
{

    public interface IDbFactory:IDisposable
    {

        [NotNull]
        DataContext Get();

        Task RunAsysnc(Action<IDbFactory> action);

        Guid SetNewContextId();

        Guid? GetCurrentContextId();
        void DisposeCurrentContext();

        IList<T> QueryProc<T>(string procedureName, params DataParameter[] parameteres);

        IList<T> ExecuteReader<T>(string sqllStatement, Func<IDataReader, T> onRead, params DataParameter[] dataParameters);
        IList<T> Query<T>(string sql, params DataParameter[] parameters);

        int ExecuteNonQuery(string sqlStatement, params DataParameter[] dataParameters);

        T ExecuteStoreProcedure<T>(string procedureName, params DataParameter[] parameteres);

        int ExecuteStoreProcedure(string procedureName, params DataParameter[] dataParameters);

        void PublishChangingEvent<TEntity>(FieldChangingEventArgs<TEntity> eventArgs);

        void PublishChangedEvent<TEntity>(FieldChangedEventArgs<TEntity> eventArgs);

        void PublishInsertingEvent<TEntity>(FieldInsertingEventArgs<TEntity> eventArgs);

        void PublishInsertedEvent<TEntity>(FieldInsertedEventArgs<TEntity> eventArgs);

        void PublishDeletingEvent<TEntity>(FieldDeletingEventArgs<TEntity> eventArgs);

        void PublishDeletedEvent<TEntity>(FieldDeletedEventArgs<TEntity> eventArgs);



    }
}
