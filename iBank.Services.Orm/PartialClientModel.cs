using System.Configuration;

namespace iBank.Services.Orm
{
    partial class iBankClientModel
    {
        private static readonly string Connection = ConfigurationManager.ConnectionStrings["FormattableConnectionString"].ConnectionString;

        public iBankClientModel(string datasource, string catalog) : base(string.Format(Connection, datasource, catalog))
        {
            this.Database.CommandTimeout = 0;
        }
    }
}
