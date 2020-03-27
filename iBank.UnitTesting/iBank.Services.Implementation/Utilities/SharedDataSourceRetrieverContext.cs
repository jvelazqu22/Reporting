using System.Collections.Generic;
using System.Linq;

using iBank.Entities.MasterEntities;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Moq;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities
{
    public class SharedDataSourceRetrieverContext
    {
        public string CorpAcctName { get; set; }
        
        public IList<CorpAcctNbrs> CorpAcctNbrs { get; set; }
        public IList<mstragcy> MasterAgency { get; set; }
        public IList<iBankDatabases> iBankDatabases { get; set; }

        public SharedDataSourceRetrieverContext()
        {
            CorpAcctNbrs = new List<CorpAcctNbrs>();
            MasterAgency = new List<mstragcy>();
            iBankDatabases = new List<iBankDatabases>();
        }

        public Mock<IQueryable<T>> GetListAsQueryable<T>(IList<T> list)
        {
            var mock = new Mock<IQueryable<T>>();
            mock.SetupIQueryable(list.AsQueryable());
            return mock;
        }
    }
} 
