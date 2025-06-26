


namespace CMS.Infrastructure.DataAccess.DbInteractions.Events
{
    public class FieldChangedEventArgs<TEntity>
    {
        public FieldChangedEventArgs<TEntity[]> EventItems { get; set; }
    }

    public delegate void FieldChangedEventHandler<TEntity>(object sender, FieldChangedEventArgs<TEntity> eventArgs);
}
