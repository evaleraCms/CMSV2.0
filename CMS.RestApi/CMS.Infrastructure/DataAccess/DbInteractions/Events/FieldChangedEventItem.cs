

namespace CMS.Infrastructure.DataAccess.DbInteractions.Events
{
    public class FieldChangedEventItem<TEntity>
    {
        public TEntity Entity { get; set; }
        public string[] FieldsChanged { get; set; }
    }
}
