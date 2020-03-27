using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.Shared
{
    [TestClass]
    public class PowerMacroHandlingTests
    {
        [TestMethod]
        public void UserMacrosNotEnabled()
        {
            var globals = PowerMacroGlobals();
            globals.Agency = "DEMO";
            globals.UserNumber = 0;
            globals.SetParmValue(WhereCriteria.POWERMACRO, "142");
            Assert.IsTrue(!PowerMacroHandling.RunMacros(globals, string.Empty));
        }

        [TestMethod]
        public void AgencyMacrosNotEnabled()
        {
            var globals = PowerMacroGlobals();
            globals.Agency = "";
            globals.UserNumber = 1592;
            globals.SetParmValue(WhereCriteria.POWERMACRO, "142");
            Assert.IsTrue(!PowerMacroHandling.RunMacros(globals, string.Empty));
        }

        [TestMethod]
        public void MacroNotFound()
        {
            var globals = PowerMacroGlobals();
            globals.Agency = "DEMO";
            globals.UserNumber = 1592;
            globals.SetParmValue(WhereCriteria.POWERMACRO, "0");
            Assert.IsTrue(!PowerMacroHandling.RunMacros(globals, string.Empty));
        }

        public ReportGlobals PowerMacroGlobals()
        {
            var globals = new ReportGlobals();
            globals.AgencyInformation.ServerName = "192.168.14.122";
            globals.AgencyInformation.DatabaseName = "ibankdemo";
            return globals;
       }

    }
}
