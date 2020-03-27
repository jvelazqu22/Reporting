using System.Collections.Generic;
using iBank.BroadcastServer.QueueManager.BuildQueue;
using iBank.Entities.ClientEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace iBank.UnitTesting.BroadcastServer.QueueManager
{
    [TestClass]
    public class DuplicateRemoverHandlerTest
    {
        [TestMethod]
        public void RemoveDuplicates_6TotalRecsAnd2Dups_Only5RecsLeft()
        {
            // Arrange
            var batches = new List<ibbatch>()
            {
                new ibbatch(){ batchnum = 1, agency = "agency1", batchname = "name1", UserNumber = 1 },
                new ibbatch(){ batchnum = 1, agency = "agency1", batchname = "name1", UserNumber = 1 },
                new ibbatch(){ batchnum = 1, agency = "agency2", batchname = "name2", UserNumber = 1 },
                new ibbatch(){ batchnum = 3, agency = "agency3", batchname = "name3", UserNumber = 3 },
                new ibbatch(){ batchnum = 4, agency = "agency4", batchname = "name4", UserNumber = 4 },
                new ibbatch(){ batchnum = 5, agency = "agency5", batchname = "name5", UserNumber = 5 },
            };
            var totalBefore = batches.Count;
            var totalAfter = 0;

            // Assert
            new DuplicateRemoverHandler().RemoveDuplicates(batches);
            totalAfter = batches.Count;

            // Act
            Assert.AreEqual(6, totalBefore);
            Assert.AreEqual(5, totalAfter);
        }
    }
}
