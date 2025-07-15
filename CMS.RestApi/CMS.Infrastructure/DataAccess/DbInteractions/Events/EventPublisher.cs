using CMS.Infrastructure.DataAccess.DbInteractions.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess.DbInteractions.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IEventSubscriptionService _eventSubscriptionService;

        public EventPublisher(IEventSubscriptionService eventSubscriptionService)
        {
            _eventSubscriptionService = eventSubscriptionService;
        }
        public void Publish<T>(T eventMassage)
        {
            var subscriptions = _eventSubscriptionService.GetSubscriptions<T>().ToList();
            subscriptions.ForEach(subscription => subscription.Handle(eventMassage));
        }
    }
}
