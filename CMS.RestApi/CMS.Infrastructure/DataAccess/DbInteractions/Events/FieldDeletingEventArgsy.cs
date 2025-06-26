
namespace CMS.Infrastructure.DataAccess.DbInteractions.Events
{
    public class FieldDeletingEventArgs<TEntity>
    {
        public TEntity Entities { get; set; }
    }

    public delegate void FieldDeletingEventHandler<TEntity>(object sender, FieldDeletingEventArgs<TEntity> eventArgs);
}
