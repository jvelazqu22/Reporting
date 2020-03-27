namespace iBank.IntegrationTests.TestTools.DatabaseGeneration
{
    public class iBankDemoGenerator : IntegrationTestBase
    {
        private readonly string _iBankDemoDatabaseGeneration = SetPath(@"DatabaseGenerationScripts\ibankdemo\ibankdemo_database.sql");

        private readonly string _iBankDemoSchemaGeneration = SetPath(@"DatabaseGenerationScripts\ibankdemo\ibankdemo_schema.sql");

        private readonly string _iBankDemoDataDirectory = SetPath(@"DatabaseGenerationScripts\ibankdemo\data_generation");

        public void GenerateNewiBankDemoDatabase()
        {
            //create the database
            Create(_iBankDemoDatabaseGeneration);

            //create the schema
            Create(_iBankDemoSchemaGeneration);

            //create the data
            CreateFromDirectory(_iBankDemoDataDirectory);
        }
    }
}
