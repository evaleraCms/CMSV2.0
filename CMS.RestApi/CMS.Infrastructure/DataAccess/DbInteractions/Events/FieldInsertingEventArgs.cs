

namespace CMS.Infrastructure.DataAccess.DbInteractions.Events
{
    public class FieldInsertingEventArgs<TEntity>
    {
        public TEntity Entities { get; set; }
    }

    public delegate void FieldInsertingEventHandler<TEntity>(object sender, FieldInsertingEventArgs<TEntity> eventArgs);
}
