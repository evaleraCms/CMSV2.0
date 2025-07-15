using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess.DbInteractions.Events.Interfaces
{
    public interface IEventSubscriptionService
    {
        ICollection<IEventConsumer<TEventMessage>> GetSubscriptions<TEventMessage>();
    }
}
