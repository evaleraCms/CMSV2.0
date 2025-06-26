

namespace CMS.Infrastructure.DataAccess.DbInteractions.Events
{
    public class FieldInsertedEventArgs<TEntity>
    {
        public TEntity[] Entities { get; set; }
    }

    public delegate void FieldInsertedEventHandler<TEntity>(object sender, FieldInsertedEventArgs<TEntity> eventArgs);
}
