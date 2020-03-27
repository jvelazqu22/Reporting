using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CODE.Framework.Core.Utilities.Extensions;

using Domain.Helper;
using Domain.Interfaces;
using Domain.Orm.iBankClientQueries.Udids;

using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Utilities.ClientData
{
    public class TripUdidRetriever
    {
        public virtual IList<UdidRecord> GetUdids<T>(IList<T> data, ReportGlobals globals, bool isReservationReport) where T : IRecKey
        {
            //get the unique reckeys from data
            var recKeys = data.Select(s => s.RecKey).Distinct().ToList();

            //get the udid numbers from the parameters
            var udidNumbers = globals.MultiUdidParameters.Parameters.Select(s => s.FieldName.TryIntParse(-1)).Where(s => s > 0).ToList();

            //get the udids that match up with the reckeys
            List<UdidRecord> udids = new List<UdidRecord>();
            if (globals.ClientType == ClientType.Sharer)
            {
                var dataSources = globals.CorpAccountDataSources.Select(x => x.DataSource);

                foreach (var source in dataSources)
                {
                    var db = new iBankClientQueryable(source.ServerAddress, source.DatabaseName);
                    var query = new GetUdidsByReckeyAndUdidNumbersQuery(db, recKeys, udidNumbers, isReservationReport);
                    udids.AddRange(query.ExecuteQuery());
                }
            }
            else
            {
                var db = new iBankClientQueryable(globals.AgencyInformation.ServerName, globals.AgencyInformation.DatabaseName);
                var query = new GetUdidsByReckeyAndUdidNumbersQuery(db, recKeys, udidNumbers, isReservationReport);
                udids = query.ExecuteQuery().ToList();
            }

            return udids;
        }
    }
}
