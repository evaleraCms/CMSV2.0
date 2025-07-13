

namespace CMS.Infrastructure.DataAccess.DbInteractions.Events
{
    public class FieldDeletedEventArgs<TEntity>
    {
        public TEntity[] Entities { get; set; }
    }

    public delegate void FieldDeletedEventHandler<TEntity>(object sender, FieldDeletedEventArgs<TEntity> eventArgs);
}
