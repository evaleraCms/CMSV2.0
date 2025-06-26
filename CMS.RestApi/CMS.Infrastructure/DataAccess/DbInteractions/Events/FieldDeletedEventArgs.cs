

namespace CMS.Infrastructure.DataAccess.DbInteractions.Events
{
    public class FieldIDeletedEventArgs<TEntity>
    {
        public TEntity[] Entities { get; set; }
    }

    public delegate void FieldDeletedEventHandler<TEntity>(object sender, FieldIDeletedEventArgs<TEntity> eventArgs);
}
