namespace iBank.Services.Orm.Databases
{
    public class iBankClientCommandDb : AbstractCommandDb
    {
        private string ServerName { get; }
        private string DatabaseName { get; }

        public iBankClientCommandDb(string serverName, string databaseName)
        {
            ServerName = serverName;
            DatabaseName = databaseName;
            Context = new iBankClientModel(serverName, databaseName);
        }

        public override object Clone()
        {
            return new iBankClientCommandDb(ServerName, DatabaseName);
        }
    }
}
