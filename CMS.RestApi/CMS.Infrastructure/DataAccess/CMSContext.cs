using CMS.Infrastructure.Common;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess
{
    public class CMSContext:DataContext
    {
        private DataOptions _options;
        public DataContextTransaction currentTransaction { get; private set; }
        public CMSContext(DataOptions options):base(options)  
        {
            _options = options;
        }

        protected override DataConnection CreateDataConnection(DataOptions options)
        {

            var conn = new CMSConnection(options);

            
            return conn;
        }

        public override DataContextTransaction BeginTransaction(IsolationLevel level)
        {
            currentTransaction = base.BeginTransaction(level);
            return currentTransaction;
        }



    }

    public class CMSConnection : DataConnection 
    {
        private const string TransationIsolationLevel = "SET TRANSACTIO ISOLATION LEVEL READ UNCOMMITTED;";
        private Dictionary<Guid, List<int>> __withNoLockHandled;

        public CMSConnection(DataOptions options):base(options) 
        {

            CommandTimeout = 100;
            __withNoLockHandled = new Dictionary<Guid, List<int>>();
        }

        public CMSConnection(DbConnection connection) : base(connection.ConnectionString)
        {

            CommandTimeout = 100;
           
        }
        //TODO
        protected override SqlStatement ProcessQuery(SqlStatement statement, EvaluationContext context)
        {
            //todo, we are working with mysql db on this data layer. no special code for sql

            return base.ProcessQuery(statement, context);
        }

        //public CMSConnection(IDataProvider dataProvider, IDbConnection connection) : base(dataProvider, connection)
        //{
        //    CommandTimeout = 180;
        //}
        private void SetWithNoLock(Guid queryId, SqlTableSource table) 
        {
            if (table.Source.SqlTableType == SqlTableType.Table) 
            {   
                if (table.Source is SelectQuery selectQuery)
                {

                    SetWithNoLock(queryId, selectQuery);
                }
            }
        
        }
        private void SetWithNoLock(Guid queryId, SelectQuery selectQuery) 
        {
            SetWithNoLock(queryId, selectQuery?.From);
            SetWithNoLock(queryId, selectQuery?.Where);
            SetWithNoLock(queryId, selectQuery?.Select);

        }

        private void SetWithNoLock(Guid queryId, SqlSelectClause selectClause)
        { 
            if(selectClause == null) return;
            var columns = selectClause.Columns;
            foreach (var column in columns) 
            {
                if (column.Expression is SqlSearchCondition searhCondition) {

                    foreach (var condition in searhCondition.Conditions) 
                    {
                        if (condition.Predicate is SqlPredicate.FuncLike sqlFuncLike) {

                            foreach (var parameter in sqlFuncLike.Function.Parameters)
                            {
                                if (parameter is SelectQuery selectQuery) {
                                    SetWithNoLock(queryId, selectQuery);
                                }
                            }
                        }
                    
                    }
                }
            
            }
        
        }

        private void SetWithNoLock(Guid queryId, SqlWhereClause whereClause)
        {
            if(whereClause == null) return;
            var conditions = whereClause?.SearchCondition?.Conditions ?? new List<SqlCondition>();
            foreach (var condition in conditions) 
            {
                if (condition.Predicate is SqlPredicate.FuncLike sqlFuncLike)
                {
                    foreach (var parameter in sqlFuncLike.Function.Parameters)
                    {
                        if (parameter is SelectQuery selectQuery) 
                        {
                            SetWithNoLock(queryId,selectQuery);
                        }
                    }
                
                }
            
            }        
        }

        private void SetWithNoLock(Guid queryId, SqlFromClause sqlfromClause)
        {
            if (sqlfromClause == null) return;
            var tables = sqlfromClause?.Tables ?? new List<SqlTableSource>();
            foreach (var table in tables) 
            {
                SetWithNoLock(queryId, table);
                foreach(var join in table.Joins)
                {
                    SetWithNoLock(queryId, join.Table);
                }
            
            }

        }

        
    
    }
}
