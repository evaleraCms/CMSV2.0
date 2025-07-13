using CMS.Infrastructure.DataAccess.DbInteractions.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess.DbInteractions.Events
{
    public class EventSubscriptionService : IEventSubscriptionService
    {

        private readonly IServiceProvider _contenerManager;

        public EventSubscriptionService(IServiceProvider contenerManager)
        {
            _contenerManager = contenerManager ;
        }
        public ICollection<IEventConsumer<TEventMessage>> GetSubscriptions<TEventMessage>()
        {
            return _contenerManager.GetServices<IEventConsumer<TEventMessage>>().ToList();
        }
    }
}
