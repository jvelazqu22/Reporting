using Domain.Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

using iBank.BroadcastServer.BroadcastBatch;
using iBank.Entities.MasterEntities;

namespace iBank.UnitTesting.iBank.BroadcastServer.Queue
{
    [TestClass]
    public class QueueFilterTests
    {
        private IList<bcstque4> _batches;
        [TestMethod]
        public void ReorderBatchesPutOfflineFirst_AllBatchesReturnOfflineIsFirstRestOrderedByNextRunSeqNo()
        {
            var filter = new QueueFilter();
            var expected = new List<bcstque4>
            {
                //offline
                new bcstque4
                    {
                        batchname = BroadcastCriteria.OfflineRecord + "offline",
                        outputdest = "2",
                        bcstseqno = 1,
                        nextrun = new DateTime(2002, 1, 1)
                    },
                //hot broadcast
                new bcstque4
                    {
                        batchname = "hot",
                        outputdest = BroadcastCriteria.EffectsOutputDest,
                        bcstseqno = 1,
                        nextrun = new DateTime(2000, 1, 1)
                    },
                //standard
                new bcstque4
                    {
                        batchname = "standard",
                        outputdest = "1",
                        bcstseqno = 1,
                        nextrun = new DateTime(2001, 1, 1)
                    }
            };

            var output = filter.ReorderBatchesToPutOfflineFirst(_batches);

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].batchname, output[i].batchname);
            }
        }
        
        [TestInitialize]
        public void Initialize()
        {
            //need batchname, outputdest, bcstseqno, nextrun
            _batches = new List<bcstque4>
            {
                //hot broadcast
                new bcstque4
                    {
                        batchname = "hot",
                        outputdest = BroadcastCriteria.EffectsOutputDest,
                        bcstseqno = 1,
                        nextrun = new DateTime(2000, 1, 1)
                    },
                //standard
                new bcstque4
                    {
                        batchname = "standard",
                        outputdest = "1",
                        bcstseqno = 1,
                        nextrun = new DateTime(2001, 1, 1)
                    },
                //offline
                new bcstque4
                    {
                        batchname = BroadcastCriteria.OfflineRecord + "offline",
                        outputdest = "2",
                        bcstseqno = 1,
                        nextrun = new DateTime(2002, 1, 1)
                    }
            };
        }
    }
}
