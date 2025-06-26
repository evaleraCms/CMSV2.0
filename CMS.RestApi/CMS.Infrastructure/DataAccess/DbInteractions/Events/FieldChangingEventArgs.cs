

namespace CMS.Infrastructure.DataAccess.DbInteractions.Events
{
    public class FieldChangingEventArgs<TEntity>
    {
        public FieldChangedEventItem<TEntity>[] EventItems { get; set; }
    }

    public delegate void FieldChangingEventHandler<TEntity>(object sender, FieldChangedEventArgs<TEntity>eventArgs);
}
