using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Helper;
using iBank.Repository.SQL.Interfaces;

namespace Domain.Orm.Classes
{
    public class BroadcastServerConfiguration
    {
        public int ServerNumber { get; set; }

        public BroadcastServerFunction Function { get; set; }

        public IList<BroadcastServerConfiguration> BuildConfiguration(IAdministrationQueryable db)
        {
            //var query = new GetBroadcastServerConfigurationsQuery(db);
            throw new NotImplementedException();
            

        }
    }
}
