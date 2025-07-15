using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess.DbInteractions.Mapping
{
    public interface IEntityBuilder
    {
        void MapEntity(FluentMappingBuilder builder);
    }
}
