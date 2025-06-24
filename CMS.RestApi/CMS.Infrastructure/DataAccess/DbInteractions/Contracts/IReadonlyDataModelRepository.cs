using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess.DbInteractions.Contracts
{
    public interface IReadonlyDataModelRepository<T> where T : class
    {
        T Get(long id);
        T Get(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] paths);

         
    }
}
