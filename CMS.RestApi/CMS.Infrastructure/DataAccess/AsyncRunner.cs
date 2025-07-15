using CMS.Infrastructure.DataAccess.DbInteractions.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess
{
   
    public class AsyncRunner : IAsyncRunner
    {
        private readonly IServiceProvider _container;
        public AsyncRunner(IServiceProvider container)
        {
            _container = container;
        }
        public Task Run(Action<IServiceProvider> action)
        {
            return Task.Run(() =>
            {
                using (var scopeContainer = _container.CreateScope())
                {
                    action(scopeContainer.ServiceProvider);
                }
            });

        }
    }
}
