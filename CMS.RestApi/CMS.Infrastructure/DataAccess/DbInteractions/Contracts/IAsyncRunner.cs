using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess.DbInteractions.Contracts
{
    public interface IAsyncRunner
    {
        Task Run(Action<IServiceProvider> action);
    }
}
