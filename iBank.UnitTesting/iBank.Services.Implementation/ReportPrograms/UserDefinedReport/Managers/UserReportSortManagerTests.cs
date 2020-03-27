using Domain.Helper;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport.Managers
{
    [TestClass]
    public class UserReportSortManagerTests
    {
        private UserReportSortManager _manager;

        [TestMethod]
        public void UserReportSortManager_SortByFirstName()
        {
            //Arrange
            var rows = new List<List<string>>
                           {
                               new List<string> {"Brad", "5/10/2016", "3", "Brad 5/10/2016 3", "True" },
                               new List<string> {"Andy", "5/11/2016", "1", "Andy 5/12/2016 1", "True" },
                               new List<string> {"Charles", "5/12/2016", "2", "Charles 5/12/2016 2", "True" }
                           };
            var columns = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Order = 1, Sort = 1, ColumnType = "TEXT", Name = "FirstName" },
                new UserReportColumnInformation {Order = 2, Sort = 0, ColumnType = "DATE", Name = "DateOfBirth" },
                new UserReportColumnInformation {Order = 3, Sort = 0, ColumnType = "NUMERIC", Name = "Age" },
                new UserReportColumnInformation {Order = 4, Sort = 0, ColumnType = "TEXT", Name = "SORT" }
            };
            _manager = new UserReportSortManager(rows, columns, DestinationSwitch.ClassicPdf);

            var expected  = new List<List<string>>
            {
                new List<string> {"Andy", "5/11/2016", "1", "Andy 5/12/2016 1", "True" },
                new List<string> {"Brad", "5/10/2016", "3", "Brad 5/10/2016 3", "True" },
                new List<string> {"Charles", "5/12/2016", "2", "Charles 5/12/2016 2", "True" }
            };

            //Act
            var output = _manager.Sort();

            //Assert
            Assert.AreEqual(true, IsEqual(expected[0], output[0]), "Error in position 0");
            Assert.AreEqual(true, IsEqual(expected[1], output[1]), "Error in position 1");
            Assert.AreEqual(true, IsEqual(expected[2], output[2]), "Error in position 2");
        }

        [TestMethod]
        public void UserReportSortManager_NoSortDefined()
        {
            //Arrange
            var rows = new List<List<string>>
                           {
                               new List<string> {"3", "Brad", "5/10/2016", "3 Brad 5/10/2016", "True" },
                               new List<string> {"1", "Andy", "5/11/2016", "1 Andy 5/11/2016", "True" },
                               new List<string> {"2", "Charles", "5/12/2016", "2 Charles 5/12/2016", "True" }
                           };
            var columns = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Order = 1, Sort = 0, ColumnType = "NUMERIC", Name = "Age" },
                new UserReportColumnInformation {Order = 2, Sort = 0, ColumnType = "TEXT", Name = "FirstName" },
                new UserReportColumnInformation {Order = 3, Sort = 0, ColumnType = "DATE", Name = "DateOfBirth" },
                new UserReportColumnInformation {Order = 4, Sort = 0, ColumnType = "TEXT", Name = "SORT" }
            };
            _manager = new UserReportSortManager(rows, columns, DestinationSwitch.ClassicPdf);

            var expected = new List<List<string>>
            {
                new List<string> {"1", "Andy", "5/11/2016", "1 Andy 5/11/2016", "True" },
                new List<string> {"2", "Charles", "5/12/2016", "2 Charles 5/12/2016", "True" },
                new List<string> {"3", "Brad", "5/10/2016", "3 Brad 5/10/2016", "True" }
            };

            //Act
            var output = _manager.Sort();

            //Assert
            Assert.AreEqual(true, IsEqual(expected[0], output[0]), "Error in position 0");
            Assert.AreEqual(true, IsEqual(expected[1], output[1]), "Error in position 1");
            Assert.AreEqual(true, IsEqual(expected[2], output[2]), "Error in position 2");
        }

        [TestMethod]
        public void UserReportSortManager_SortByDateOfBirth()
        {
            //Arrange
            var rows = new List<List<string>>
                           {
                               new List<string> {"Brad", "5/10/2016", "3", "5/10/2016 Brad 3", "True" },
                               new List<string> {"Andy", "5/11/2016", "1", "5/12/2016 Andy 1", "True" },
                               new List<string> {"Charles", "5/12/2016", "2", "5/12/2016 Charles 2", "True" }

                           };
            var columns = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Order = 1, Sort = 0, ColumnType = "TEXT", Name = "FirstName" },
                new UserReportColumnInformation {Order = 2, Sort = 1, ColumnType = "DATE", Name = "DateOfBirth" },
                new UserReportColumnInformation {Order = 3, Sort = 0, ColumnType = "NUMERIC", Name = "Age" },
                new UserReportColumnInformation {Order = 4, Sort = 0, ColumnType = "TEXT", Name = "SORT" }
            };

           _manager = new UserReportSortManager(rows, columns, DestinationSwitch.ClassicPdf);

            var expected = new List<List<string>>
            {
                    new List<string> {"Brad", "5/10/2016", "3", "5/10/2016 Brad 3", "True" },
                    new List<string> {"Andy", "5/11/2016", "1", "5/12/2016 Andy 1", "True" },
                    new List<string> {"Charles", "5/12/2016", "2", "5/12/2016 Charles 2", "True" }
            };
            //Act
            var output = _manager.Sort();

            //Assert
            Assert.AreEqual(true, IsEqual(expected[0], output[0]), "Error in position 0");
            Assert.AreEqual(true, IsEqual(expected[1], output[1]), "Error in position 1");
            Assert.AreEqual(true, IsEqual(expected[2], output[2]), "Error in position 2");
        }

        [TestMethod]
        public void UserReportSortManager_SortByAge()
        {
            //Arrange
            var rows = new List<List<string>>
                           {
                               new List<string> {"Brad", "5/10/2016", "3", "3 Brad 5/10/2016", "True" },
                               new List<string> {"Andy", "5/11/2016", "1", "1 Andy 5/12/2016", "True" },
                               new List<string> {"Charles", "5/12/2016", "2", "2 Charles 5/12/2016", "True" }
                           };
            var columns = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Order = 1, Sort = 0, ColumnType = "TEXT", Name = "FirstName" },
                new UserReportColumnInformation {Order = 2, Sort = 0, ColumnType = "DATE", Name = "DateOfBirth" },
                new UserReportColumnInformation {Order = 3, Sort = 1, ColumnType = "NUMERIC", Name = "Age" },
                new UserReportColumnInformation {Order = 4, Sort = 0, ColumnType = "TEXT", Name = "SORT" }
            };

            var manager = new UserReportSortManager(rows, columns, DestinationSwitch.ClassicPdf);

            var expected = new List<List<string>>
            {
                    new List<string> {"Andy", "5/11/2016", "1", "1 Andy 5/12/2016", "True" },
                    new List<string> {"Charles", "5/12/2016", "2", "2 Charles 5/12/2016", "True" },
                    new List<string> {"Brad", "5/10/2016", "3", "3 Brad 5/10/2016", "True" }
            };
            //Act
            var output = manager.Sort();

            //Assert
            Assert.AreEqual(true, IsEqual(expected[0], output[0]), "Error in position 0");
            Assert.AreEqual(true, IsEqual(expected[1], output[1]), "Error in position 1");
            Assert.AreEqual(true, IsEqual(expected[2], output[2]), "Error in position 2");
        }

        [TestMethod]
        public void UserReportSortManager_SortByFirstNameThenByAge()
        {
            //Arrange
            var rows2 = new List<List<string>>
                            {
                                new List<string> {"Brad", "5/10/2016", "3", "3 Brad 5/10/2016", "False" },
                                new List<string> {"Brad", "5/11/2016", "1", "1 Brad 5/11/2016", "True" },
                                new List<string> {"Brad", "5/12/2016", "2", "2 Brad 5/12/2016", "False" },
                                new List<string> {"Andy", "5/11/2016", "1", "1 Andy 5/11/2016", "True" },
                                new List<string> {"Charles", "5/12/2016", "2", "2 Charles 5/12/2016", "True" }
                            };
            var columns = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Order = 1, Sort = 2, ColumnType = "TEXT", Name = "FirstName" },
                new UserReportColumnInformation {Order = 2, Sort = 0, ColumnType = "DATE", Name = "DateOfBirth" },
                new UserReportColumnInformation {Order = 3, Sort = 1, ColumnType = "NUMERIC", Name = "Age" },
                new UserReportColumnInformation {Order = 4, Sort = 0, ColumnType = "TEXT", Name = "SORT" }
            };

            var manager = new UserReportSortManager(rows2, columns, DestinationSwitch.ClassicPdf);

            var expected = new List<List<string>>
            {
                new List<string> {"Andy", "5/11/2016", "1", "1 Andy 5/11/2016", "True" },
                new List<string> {"Brad", "5/11/2016", "1", "1 Brad 5/11/2016", "True" },
                new List<string> {"Brad", "5/12/2016", "2", "2 Brad 5/12/2016", "False" },
                new List<string> {"Charles", "5/12/2016", "2", "2 Charles 5/12/2016", "True" },
                new List<string> {"Brad", "5/10/2016", "3", "3 Brad 5/10/2016", "False" }

            };
            //Act
            var output = manager.Sort();

            //Assert
            Assert.AreEqual(true, IsEqual(expected[0], output[0]), "Error in position 0");
            Assert.AreEqual(true, IsEqual(expected[1], output[1]), "Error in position 1");
            Assert.AreEqual(true, IsEqual(expected[2], output[2]), "Error in position 2");
            Assert.AreEqual(true, IsEqual(expected[3], output[3]), "Error in position 3");
            Assert.AreEqual(true, IsEqual(expected[4], output[4]), "Error in position 4");
        }

        [TestMethod]
        public void UserReportSortManager_SortByFirstNameThenByDateOfBirth()
        {
            //Arrange
            var rows2 = new List<List<string>>
                            {
                                new List<string> {"Brad", "5/10/2016", "3", "Brad 5/10/2016 3", "False" },
                                new List<string> {"Brad", "5/11/2016", "1", "Brad 5/11/2016 1", "True"},
                                new List<string> {"Brad", "5/12/2016", "2","Brad 5/12/2016 2", "False"},
                                new List<string> {"Andy", "5/11/2016", "1","Andy 5/11/2016 1", "True" },
                                new List<string> {"Charles", "5/12/2016", "2", "Charles 5/12/2016", "True" }
                            };
            var columns = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Order = 1, Sort = 1, ColumnType = "TEXT", Name = "FirstName" },
                new UserReportColumnInformation {Order = 2, Sort = 2, ColumnType = "DATE", Name = "DateOfBirth" },
                new UserReportColumnInformation {Order = 3, Sort = 0, ColumnType = "NUMERIC", Name = "Age" },
                new UserReportColumnInformation {Order = 4, Sort = 0, ColumnType = "TEXT", Name = "SORT" }
            };

            var manager = new UserReportSortManager(rows2, columns, DestinationSwitch.ClassicPdf);

            var expected = new List<List<string>>
            {
                new List<string> {"Andy", "5/11/2016", "1","Andy 5/11/2016 1", "True" },
                new List<string> {"Brad", "5/10/2016", "3", "Brad 5/10/2016 3", "False" },
                new List<string> {"Brad", "5/11/2016", "1", "Brad 5/11/2016 1", "True"},
                new List<string> {"Brad", "5/12/2016", "2","Brad 5/12/2016 2", "False"},
                new List<string> {"Charles", "5/12/2016", "2", "Charles 5/12/2016", "True" }
            };
            //Act
            var output = manager.Sort();

            //Assert
            Assert.AreEqual(true, IsEqual(expected[0], output[0]), "Error in position 0");
            Assert.AreEqual(true, IsEqual(expected[1], output[1]), "Error in position 1");
            Assert.AreEqual(true, IsEqual(expected[2], output[2]), "Error in position 2");
            Assert.AreEqual(true, IsEqual(expected[3], output[3]), "Error in position 3");
            Assert.AreEqual(true, IsEqual(expected[4], output[4]), "Error in position 4");
        }

        [TestMethod]
        public void UserReportSortManager_SortByAgeThenByFirstNameThenByDateOfBirth()
        {
            //Arrange
            var rows2 = new List<List<string>>
                            {
                                new List<string> {"Brad", "5/10/2016", "3", "Brad 5/10/2016 3", "False" },
                                new List<string> {"Brad", "5/11/2016", "1", "Brad 5/11/2016 1", "True"},
                                new List<string> {"Brad", "5/12/2016", "2","Brad 5/12/2016 2", "False"},
                                new List<string> {"Andy", "5/11/2016", "1","Andy 5/11/2016 1", "True" },
                                new List<string> {"Charles", "5/12/2016", "2", "Charles 5/12/2016", "True" }

                            };
            var columns = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Order = 1, Sort = 1, ColumnType = "TEXT", Name = "FirstName" },
                new UserReportColumnInformation {Order = 2, Sort = 2, ColumnType = "DATE", Name = "DateOfBirth" },
                new UserReportColumnInformation {Order = 3, Sort = 3, ColumnType = "NUMERIC", Name = "Age" },
                new UserReportColumnInformation {Order = 4, Sort = 0, ColumnType = "TEXT", Name = "SORT" }
            };

            var manager = new UserReportSortManager(rows2, columns, DestinationSwitch.ClassicPdf);

            var expected = new List<List<string>>
            {
                new List<string> {"Andy", "5/11/2016", "1","Andy 5/11/2016 1", "True" },
                new List<string> {"Brad", "5/10/2016", "3", "Brad 5/10/2016 3", "False" },
                new List<string> {"Brad", "5/11/2016", "1", "Brad 5/11/2016 1", "True"},
                new List<string> {"Brad", "5/12/2016", "2","Brad 5/12/2016 2", "False"},
                new List<string> {"Charles", "5/12/2016", "2", "Charles 5/12/2016", "True" }
            };
            //Act
            var output = manager.Sort();

            //Assert
            Assert.AreEqual(true, IsEqual(expected[0], output[0]), "Error in position 0");
            Assert.AreEqual(true, IsEqual(expected[1], output[1]), "Error in position 1");
            Assert.AreEqual(true, IsEqual(expected[2], output[2]), "Error in position 2");
            Assert.AreEqual(true, IsEqual(expected[3], output[3]), "Error in position 3");
            Assert.AreEqual(true, IsEqual(expected[4], output[4]), "Error in position 4");
        }

        private bool IsEqual(List<string> expected, List<string> output)
        {
            if (!expected[0].Equals(output[0])) return false;

            if (!expected[1].Equals(output[1])) return false;

            if (!expected[2].Equals(output[2])) return false;

            return true;
        }

    }
}
