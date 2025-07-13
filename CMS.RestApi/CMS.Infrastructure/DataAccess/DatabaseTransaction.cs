using CMS.Infrastructure.DataAccess.DbInteractions.Contracts;
using LinqToDB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess
{
    public class DatabaseTransaction : ITransaction, IDisposable
    {

        private readonly IDbFactory _dbFactory;
        protected DataContext DataContext => _dbFactory.Get();
        protected DataContextTransaction Transaction;
        private bool _isDisposed;
        public Guid InstanceId { get; set; }
        private readonly ConcurrentDictionary<Guid, int> _contextTrasactionCount;

        public DatabaseTransaction(IDbFactory datasetFactory)
        {
            _dbFactory = datasetFactory;
            InstanceId = Guid.NewGuid();
            _contextTrasactionCount = new ConcurrentDictionary<Guid, int>();
        }
        public void Commit()
        {
            if (Transaction == null)
            {
                CreateTransaction(IsolationLevel.Unspecified);
            }
            var contextId = _dbFactory.GetCurrentContextId();
            if (contextId.HasValue) 
            {
                if (_contextTrasactionCount.ContainsKey(contextId.Value))
                { 
                    int count = _contextTrasactionCount[contextId.Value];   
                    Transaction?.CommitTransaction();
                    CloseConnection();
                    Debug.WriteLine($"Transaction committed for context {contextId.Value} with count {count}");
                }
            
            }
        }

        private void CreateTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified,DateTime? timeOut = null)
        {

            if (Transaction == null)
            {
                var contextId = _dbFactory.SetNewContextId();
                _contextTrasactionCount.TryAdd(contextId, 1);
                Transaction = DataContext.BeginTransaction(isolationLevel);
                _isDisposed = false;

                Debug.WriteLine($"DatabaseTrasnaction {InstanceId} Context {contextId} connection: count Trasaction {_contextTrasactionCount[contextId]}");

            }
            else { 
                var contextId = _dbFactory.GetCurrentContextId();
                if (contextId.HasValue) {

                    _contextTrasactionCount[contextId.Value]++;
                    Debug.WriteLine($"DatabaseTrasnaction {InstanceId} Context {contextId} connection: count Trasaction {_contextTrasactionCount[contextId.Value]}");

                }

            }

        }

        public void CloseConnection() {
            var contextId = _dbFactory.GetCurrentContextId();
            if (contextId.HasValue) 
            {
                if (_contextTrasactionCount.ContainsKey(contextId.Value)) { 
                
                    int count = _contextTrasactionCount[contextId.Value];
                    Transaction = null;
                    _dbFactory.DisposeCurrentContext();
                    _contextTrasactionCount.TryRemove(contextId.Value, out int temp);

                    Debug.WriteLine($"DatabaseTrasnaction {InstanceId} Context {contextId} connection: count Trasaction {count}");
                }
             
            }

        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                return;
            }

            Transaction = null;
            _contextTrasactionCount.Clear();
            _isDisposed = true;

            Debug.WriteLine($"DatabaseTrasnaction {InstanceId} disposed.");
        }

        public void BeginTrasaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified, DateTime? timeOut = null)
        {
            CreateTransaction(isolationLevel, timeOut);
        }

        public void Rollback()
        {
            Transaction?.RollbackTransaction();
            CloseConnection();
        }
    }
}
