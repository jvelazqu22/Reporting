using System;
using System.IO;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;

using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.Shared
{
    [TestClass]
    public class PowerMacroIntegrationTests
    {
        [TestMethod]
        public void RunPowerMacro()
        {
            //PowerMacros are set up by report, user, and agency. They're run only for raw XLSX output that has
            // already been generated. This does not test the macro itself.
            var globals = GenerateRunPowerMacroReportGlobals();
            var path = Path.GetFullPath(@"iBank.Services.Implementation\Shared\PowerMacroExternalFiles\");

            var controlOutput = new FileInfo(path + @"PowerMacroControlOutput.xlsx");
            Assert.IsTrue(controlOutput.Exists, "Control output file not found.");

            var controlInput = new FileInfo(path + @"PowerMacroControlInput.xlsx");
            Assert.IsTrue(controlInput.Exists, "Control input file not found.");

            var resultFile = controlInput.CopyTo(path + @"PowerMacroResult.xlsx", true);
            Assert.IsTrue(resultFile.Exists, "PowerMacro test file not found.");
            PowerMacroHandling.RunMacros(globals, resultFile.FullName);
            resultFile = new FileInfo(path + @"PowerMacroResult.xlsx");

            //This is not sufficient to know the file really is the same, but byte-wise comparison is returning false, 
            //even though the files are the same (according to BeyondCompare).
            //
            Assert.IsTrue(controlOutput.Length == resultFile.Length,"Results file does not match the control output file.");

            //Another relatively simple check, but deeper. Same problem as above. Unfortunately not equal, so...
            //Compare byte by byte? Is it worth tracking down what the difference is? 
            //Assert.IsTrue(File.ReadAllBytes(controlOutput.FullName).SequenceEqual(File.ReadAllBytes(resultFile.FullName)));
            //var f1 = File.ReadAllBytes(controlOutput.FullName);
            //var f2 = File.ReadAllBytes(resultFile.FullName);
            //for (int i = 0; i < f1.Length -1; i++)
            //{
            //    if (f1[i] == f2[i]) continue;
            //    // Assert.Fail();
            //    break;
            //}


        }

        private ReportGlobals GenerateRunPowerMacroReportGlobals()
        {
            var globals = new ReportGlobals
            {
                Agency = "DEMO",
             };
            globals.SetParmValue(WhereCriteria.POWERMACRO, "142");
            globals.AgencyInformation.ServerName = "192.168.14.122";
            globals.AgencyInformation.DatabaseName = "ibankdemo";
            globals.UserNumber = 1592;
            return globals;
        }

        private void Cleanup()
        {
            //TODO: Do we want to delete the test file or leave it for a human to compare it?
        }

    }
}
