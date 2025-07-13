using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess.DbInteractions.Contracts
{
    public interface ITransaction
    {
        void BeginTrasaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified, DateTime? timeOut = null);
        void Commit();
        void Rollback();
        void Dispose();

    }
}
