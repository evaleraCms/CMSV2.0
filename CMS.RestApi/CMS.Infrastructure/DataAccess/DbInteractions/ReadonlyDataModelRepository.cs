using CMS.Infrastructure.DataAccess.DbInteractions.Contracts;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess.DbInteractions
{
    public class ReadonlyDataModelRepository<T> : IReadonlyDataModelRepository<T> where T : class
    {

        protected IDbFactory DatabaseFactory { get; private set; }

        protected DataContext DataContext => DatabaseFactory.Get();
        protected IQueryable<T> Table => DataContext.GetTable<T>().AsSubQuery();
        public T Get(long id)
        {
            throw new NotImplementedException();
        }

        public T Get(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] paths)
        {
            throw new NotImplementedException();
        }


    }
}
