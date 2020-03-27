using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Utilities.ReportSetup;
using iBankDomain.RepositoryInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities
{
    [TestClass]
    public class ReportStatusHandlerTests
    {
        [TestMethod]
        public void is_record_available_to_run_status_is_pending_return_true()
        {
            var sut = new ReportStatusHandler();
            var rec = new reporthandoff {parmvalue = Constants.Pending};

            var result = sut.IsRecordAvailableToRun(rec);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void is_record_available_to_run_status_not_pending_return_false()
        {
            var sut = new ReportStatusHandler();
            var rec = new reporthandoff {parmvalue = "foo"};

            var result = sut.IsRecordAvailableToRun(rec);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void is_record_available_to_run_rec_is_null_return_false()
        {
            var sut = new ReportStatusHandler();
            reporthandoff rec = null;

            var result = sut.IsRecordAvailableToRun(rec);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void set_status_to_in_process()
        {
            var sut = new ReportStatusHandler();
            var rec = new reporthandoff {svrnumber = 0, parmvalue = Constants.Pending};
            var serverNumber = 1;
            
            var mockCmd = new Mock<ICommandDb>();
            var mockStore = new Mock<IMasterDataStore>();
            mockStore.Setup(x => x.MastersCommandDb).Returns(mockCmd.Object);

            sut.SetStatusToInProcess(mockStore.Object, serverNumber, rec);

            mockCmd.Verify(x =>
                x.Update(It.Is<IList<reporthandoff>>(y =>
                    y.Any(z => z.parmvalue.Equals(Constants.InProcess) && z.svrnumber == serverNumber))), Times.Once);
        }
    }
}
