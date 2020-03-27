namespace iBank.IntegrationTests.TestTools.DatabaseGeneration
{
    public class iBankMastersGenerator : IntegrationTestBase
    {
        private readonly string _iBankMastersDatabaseGeneration = SetPath(@"DatabaseGenerationScripts\ibankmasters\ibankmasters_database.sql");

        private readonly string _iBankMastersSchemaGeneration = SetPath(@"DatabaseGenerationScripts\ibankmasters\ibankmasters_schema.sql");

        private readonly string _iBankMastersDataDirectory = SetPath(@"DatabaseGenerationScripts\ibankmasters\data_generation");
        public void GenerateNewiBankMastersDatabase()
        {
            //create the database
            Create(_iBankMastersDatabaseGeneration);

            //create the schema
            Create(_iBankMastersSchemaGeneration);

            //create the data
            CreateFromDirectory(_iBankMastersDataDirectory);
        }
    }
}
