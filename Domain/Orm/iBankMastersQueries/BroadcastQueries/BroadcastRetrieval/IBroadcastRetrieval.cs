using System.Collections.Generic;
using System.Linq;
using iBank.Entities.MasterEntities;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastRetrieval
{
    public interface IBroadcastRetrieval
    {
        List<bcstque4> GetBroadcasts(IQueryable<bcstque4> queue);
    }
}
