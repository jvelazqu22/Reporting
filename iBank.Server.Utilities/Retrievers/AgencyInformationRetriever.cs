using Domain.Orm.Classes;
using System.Collections.Generic;
using System.Linq;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.Retrievers
{
    public class AgencyInformationRetriever
    {
        public IList<AgencyInformation> GetAgencies(IList<IQuery<IList<AgencyInformation>>> getAgenciesQuery)
        {
            var validDiscreteAgencies = new List<AgencyInformation>();

            foreach (var query in getAgenciesQuery)
            {
                AddAgencies(validDiscreteAgencies, query);
            }

            return validDiscreteAgencies;
        }

        private void AddAgencies(IList<AgencyInformation> validDiscreteAgencies, IQuery<IList<AgencyInformation>> getAgencies)
        {
            var possibleAgencies = getAgencies.ExecuteQuery();
            foreach (var ag in possibleAgencies.Where(ag => !validDiscreteAgencies.Any(x => x.Agency == ag.Agency
                                                                    && x.DatabaseName == ag.DatabaseName
                                                                    && x.TimeZoneOffset == ag.TimeZoneOffset)))
            {
                validDiscreteAgencies.Add(ag);
            }
        }
    }
}
