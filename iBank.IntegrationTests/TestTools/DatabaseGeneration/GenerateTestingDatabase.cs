using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.TestTools.DatabaseGeneration
{
    [TestClass]
    public class GenerateTestingDatabase
    {
        //[AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            //generate the ibankmasters database
            var masters = new iBankMastersGenerator();
            masters.GenerateNewiBankMastersDatabase();

            //generate the demo database
            var demo = new iBankDemoGenerator();
            demo.GenerateNewiBankDemoDatabase();
        }
    }
}
