using CMS.Infrastructure.Common;
using CMS.Infrastructure.Common.Extensions;
using CMS.Infrastructure.DataAccess.DbInteractions.Contracts;
using CMS.Infrastructure.DataAccess.DbInteractions.Events;
using CMS.Infrastructure.DataAccess.DbInteractions.Events.Interfaces;
using CMS.Infrastructure.DataAccess.DbInteractions.Mapping;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using MySqlConnector;

namespace CMS.Infrastructure.DataAccess
{
    public class DbFactory : Disposable, IDbFactory
    {
        private readonly IAsyncRunner _asyncRunner;
        private readonly IEventPublisher _eventPublisher;
        private readonly int _concurrencyLevel = Environment.ProcessorCount * 2;
        [NotNull]
        private readonly ConcurrentDictionary<string, DataContext> _threads;
        protected IDataProvider LinqToDbDataProvider => LinqToDB.DataProvider.MySql.MySqlTools.GetDataProvider(ProviderName.MySqlConnector);
        protected string CurrentConnectionString => ConfigSettings.CMSConnStr;
        protected string ConfigurationName => ProviderName.MySqlConnector;


        private static readonly object LockObj = new object();
        private readonly Guid _id;
        private MappingSchema _additionalSchema;
        protected MappingSchema AdditionalSchema
        {
            get
            {
                if (!(_additionalSchema is null))
                    return _additionalSchema;
                lock (LockObj)
                {
                    var schema = new MappingSchema(ConfigurationName);
                    var types = typeof(IEntityBuilder).GetClosedTypesFromType();

                    var builder = new FluentMappingBuilder(schema);
                    foreach (var type in types)
                    {
                        IEntityBuilder entityBuilder = Activator.CreateInstance(type) as IEntityBuilder;
                        entityBuilder?.MapEntity(builder);
                    }

                    builder.Build();
                    //schema.SetConverter<DateOnly, DateTime>(dt => new DateTime(new DateTime().Ticks));

                }
                return _additionalSchema;
            }

        }

        public DbFactory(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
            _threads = new ConcurrentDictionary<string, DataContext>(_concurrencyLevel, 101);
            _id = Guid.NewGuid();

            DataConnection.TurnTraceSwitchOn();
            DataConnection.DefaultOnTraceConnection = (t) =>
            {
                StringBuilder sql = new StringBuilder();
                string sqlText = t.SqlText.IsEmpty() ? t.CommandText : t.SqlText;
                sql.Append(sqlText);
                if (t.Exception != null)
                {
                    var exception = t.Exception.GetBaseException();
                    exception.Data["Sql"] = sql.ToString();

                    Debug.WriteLine(t.Exception.Message, t.TraceLevel.ToString());

                }
                else if (t.TraceInfoStep == TraceInfoStep.BeforeExecute)
                {
                    Debug.WriteLine("____________________________________________");
                    Debug.WriteLine(sql.ToString(), t.TraceLevel.ToString());

                }
                else if (t.TraceInfoStep == TraceInfoStep.AfterExecute)
                {
                    Debug.WriteLine($"Execution Completed: {t.ExecutionTime}");

                }
                else if (t.TraceInfoStep == TraceInfoStep.MapperCreated)
                {
                    Debug.WriteLine($"Execution MapperCreated: {t.ExecutionTime}");
                }


            };


        }

        public Task RunAsync(Action<IDbFactory> action)
        {
            return _asyncRunner.Run((container) =>
            {

                var db = container.GetService<IDbFactory>();
                action((IDbFactory)db);
            });
        }

        public DataContext Get()
        {
            var db = GetDataContext();
            return db;

        }

        private DataContext GetDataContext()
        {
            var thread = Thread.CurrentThread;
            Debug.WriteLine($"DataContext {_id} ThreadId: {thread.ManagedThreadId}");
            var contextId = GetCurrentContextId()?.ToString();
            if (contextId == null)
            {
                contextId = thread.ManagedThreadId.ToString();
            }
            if (contextId != null && _threads.ContainsKey(contextId))
            {
                Debug.WriteLine($"DataContext {_id} Existing {contextId} {DateTime.Now}");
                return _threads[contextId];
            }

            var commandIntercepter = new AppCommandInterceptor();
            var options = new DataOptions();
            options = options.UseMappingSchema(AdditionalSchema);
            options = options.UseInterceptor(commandIntercepter);
            options = options.UseMySqlConnector(CurrentConnectionString);

            //todo finish the method next

            var dbContext = new CMSContext(options);
            dbContext.CloseAfterUse = true;
            if (contextId != null && !_threads.ContainsKey(contextId))
            {
                _threads.TryAdd(contextId, dbContext);
                Debug.WriteLine($"DataContext {_id} new {contextId} {DateTime.Now} Count {_threads.Count}");
            }
            else
            {
                Debug.WriteLine($"DataContext {_id} No CnotextId");

            }
            return dbContext;

        }

        private DbConnection GetDbConnection(string connectionString = null)
        {
            if (connectionString.IsEmpty())
            {
                connectionString = CurrentConnectionString;
            }

            DbConnection cn = new MySqlConnection(connectionString);

            return cn;


        }

        private void ClearContextId()
        {

            CallContext.LogicalSetData("ContextId", null);
        }

        public void DisposeCurrentContext()
        {
            var contextId = GetCurrentContextId()?.ToString();
            if (contextId != null)
            {
                DisposeContext(contextId);
                ClearContextId();
            }
        }

        public void DisposeContext(string contextId)
        {
            if (_threads.ContainsKey(contextId))
            {
                DataContext temp;
                _threads.TryRemove(contextId, out temp);
                (temp as IDataContext)?.Close();

            }

            Debug.WriteLine($"DataContext {_id} Disposed {contextId} {DateTime.Now} Count {_threads.Count}");
        }

        protected override void DisposeCore()
        {
            Debug.WriteLine($"DataContext DbFactory clean up {_threads.Count} found.");
            foreach (var thread in _threads)
            {
                DisposeContext(thread.Key);
            }
        }

        protected virtual DataConnection CreateDataConnection()
        {
            var dbConnection = GetDbConnection(CurrentConnectionString);
            var dataContext = new CMSConnection(dbConnection);
            dataContext.AddInterceptor(new AppCommandInterceptor());
            dataContext.AddMappingSchema(AdditionalSchema);
            return dataContext;
        }


        public int ExecuteNonQuery(string sqlStatement, params DataParameter[] dataParameters)
        {
            using (var dataContext = CreateDataConnection())
            {
                var command = new CommandInfo(dataContext, sqlStatement, dataParameters);
                var affectedRecords = command.Execute();
                UpdateOutputParameter(command, dataParameters);
                return affectedRecords;
            }
        }

        public IList<T> ExecuteReader<T>(string sqllStatement, Func<IDataReader, T> onRead, params DataParameter[] dataParameters)
        {
           List<T> results = new List<T>();
            using (var dataContext = CreateDataConnection())
            {
                var command = new CommandInfo(dataContext, sqllStatement, dataParameters);
                using (var reader = command.ExecuteReader())
                {
                    var datareader = reader.Reader;
                    if (datareader != null)
                    {
                        while (datareader.Read())
                        {
                            T result = onRead(datareader);

                            results.Add(result);
                        }
                    }
                }
                
            }
            return results;
        }

        public T ExecuteStoreProcedure<T>(string procedureName, params DataParameter[] parameteres)
        {
            using (var dataContext = CreateDataConnection()) 
            {
                var command = new CommandInfo(dataContext, procedureName, parameteres);
                var result = command.ExecuteProc<T>();
                UpdateOutputParameter(command, parameteres);
                return result;
            }
        }

        public int ExecuteStoreProcedure(string procedureName, params DataParameter[] dataParameters)
        {
            using(var dataContext = CreateDataConnection())
            {
                var command = new CommandInfo(dataContext, procedureName, dataParameters);
                var affectedRecords = command.ExecuteProc();
                UpdateOutputParameter(command, dataParameters);
                return affectedRecords;
            }
        }



        public Guid? GetCurrentContextId()
        {
            return CallContext.LogicalGetData("ContextId") as Guid? ?? null;
        }

        public void PublishChangedEvent<TEntity>(FieldChangedEventArgs<TEntity> eventArgs)
        {
            _eventPublisher.Publish(eventArgs);
        }

        public void PublishChangingEvent<TEntity>(FieldChangingEventArgs<TEntity> eventArgs)
        {
            _eventPublisher.Publish(eventArgs);
        }

        public void PublishDeletedEvent<TEntity>(FieldDeletedEventArgs<TEntity> eventArgs)
        {
            _eventPublisher.Publish(eventArgs);
        }

        public void PublishDeletingEvent<TEntity>(FieldDeletingEventArgs<TEntity> eventArgs)
        {
            _eventPublisher.Publish(eventArgs);
        }

        public void PublishInsertedEvent<TEntity>(FieldInsertedEventArgs<TEntity> eventArgs)
        {
            _eventPublisher.Publish(eventArgs);
        }

        public void PublishInsertingEvent<TEntity>(FieldInsertingEventArgs<TEntity> eventArgs)
        {
            _eventPublisher.Publish(eventArgs);
        }

        public IList<T> Query<T>(string sql, params DataParameter[] parameters)
        {
            using (var dataContext = CreateDataConnection())
            {
               
                return dataContext.Query<T>(sql, parameters)?.ToList()?? new List<T>();
            }
        }

        public IList<T> QueryProc<T>(string procedureName, params DataParameter[] parameteres)
        {
            var dataContext = CreateDataConnection();
            var command = new CommandInfo(dataContext, procedureName, parameteres);
            var results = command.QueryProc<T>()?.ToList() ?? new List<T>();
            UpdateOutputParameter(command, parameteres);
            return results;
        }

        public Task RunAsysnc(Action<IDbFactory> action)
        {
            return _asyncRunner.Run((container) => 
            {
                var db = container.GetService<IDbFactory>();
                action((IDbFactory)db);
            });
        }

        public Guid SetNewContextId()
        {
            var contextId = Guid.NewGuid();
            CallContext.LogicalSetData("ContextId", contextId);
            return contextId;
        }

        private void UpdateOutputParameter(CommandInfo command, DataParameter[] dataParameters)
        {
            if (dataParameters == null || dataParameters.Length == 0)
                return;
            foreach (var parameter in dataParameters.Where(x => x.Direction == ParameterDirection.Output))
            {
                UpdateParameterValue(command, parameter);
            }
        }

        private void UpdateParameterValue(CommandInfo command, DataParameter parameter) 
        { 
            if(parameter is null)
                throw new ArgumentNullException(nameof(parameter), "Parameter cannot be null.");

            if (command.Parameters.Length > 0 &&
                command.Parameters.Any(q=> q.Name == parameter.Name) &&
                command.Parameters.FirstOrDefault(q=> q.Name ==parameter.Name) is IDbDataParameter param)
            {

                parameter.Value = param.Value;
            }

        }

        
    }
}
