using System.Collections.Generic;
using Domain.Helper;
using iBank.Repository.SQL.Interfaces;
using System.Linq;
using Domain.Orm.iBankMastersQueries.Agency;
using Domain.Orm.iBankMastersQueries.Other;

namespace iBank.Services.Implementation.Utilities.ClientData
{
    public class SharedDataSourceRetriever
    {
        private IMasterDataStore MasterDataStore { get; set; }

        public SharedDataSourceRetriever(IMasterDataStore masterDataStore)
        {
            MasterDataStore = masterDataStore;
        }

        public IList<CorpAccountDataSource> GetDataSourcesForAllAgencies(string corpAcctName)
        {
            var corpAccountDataSources = new List<CorpAccountDataSource>();
            //get agencies from JunctionAgcyCorp because they match
            var agencies = GetAllAgenciesForSharedAgency(corpAcctName);

            var dataSourceQuery = new GetDataSourceAddressByAgencyQuery(MasterDataStore.MastersQueryDb, agencies);
            var dataSurces = dataSourceQuery.ExecuteQuery();
            foreach (var agency in agencies)
            {
                var corpAccountDataSource = new CorpAccountDataSource();
                corpAccountDataSource.Agency = agency.Trim();
                corpAccountDataSource.DataSource = dataSurces.FirstOrDefault(x => x.Agency == agency.Trim());
                corpAccountDataSources.Add(corpAccountDataSource);
            }
            return corpAccountDataSources;
        }

        public IList<string> GetAllAgenciesForSharedAgency(string corpAcctName)
        {
            var agenciesQuery = new GetAgenciesByJunctionAgcyCorpQuery(MasterDataStore.MastersQueryDb, corpAcctName);
            return agenciesQuery.ExecuteQuery();
        }

    }
}
