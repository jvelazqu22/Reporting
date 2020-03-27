using CODE.Framework.Services.Client;
using CODE.Framework.Wpf.Mvvm;
using CrystalDecisions.CrystalReports.Engine;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Contracts;
using iBank.Services.Implementation.Shared;
using System;

namespace iBank.WPF.ReportTester.Models.Home
{
    public class StartViewModel : ViewModel
    {
        public static StartViewModel Current { get; set; }

        public StartViewModel()
        {
            Current = this;
        }

        public void LoadActions()
        {
            Actions.Clear();

            // TODO: The following list of actions is used to populate the application's main navigation area (such as a menu or a home screen)

            Actions.Add(new ViewAction("Run Scheduled Reports", category: "Tester",
                execute: (a, o) => RunScheduledReports()) { Significance = ViewActionSignificance.AboveNormal });
            Actions.Add(new ViewAction("Run Broadcasts", category: "Tester",
                execute: (a, o) => RunBroadcasts()) { Significance = ViewActionSignificance.AboveNormal });
            Actions.Add(new ViewAction("Run One Report", category: "Tester",
                execute: (a, o) => Controller.Action("Utilities", "RunOneReport")) { Significance = ViewActionSignificance.AboveNormal });
            Actions.Add(new ViewAction("Generate POCO Class", category: "Tester", execute: (a, o) => Controller.Action("Utilities", "GenPOCO"))
            {
                Significance = ViewActionSignificance.AboveNormal
            });
            Actions.Add(new ViewAction("Generate Report Files", category: "Tester", execute: (a, o) => GenFilesFromCrystalReport())
            {
                Significance = ViewActionSignificance.AboveNormal
            });

            Actions.Add(new ViewAction("Test Shared Proc", category: "Tester", execute: (a, o) => TestSharedProc())
            {
                Significance = ViewActionSignificance.AboveNormal
            });
            Actions.Add(new ViewAction("Play with SSRS", category: "Tester", execute: (a, o) => PlayWithSSRS())
            {
                Significance = ViewActionSignificance.AboveNormal
            });
            Actions.Add(new ViewAction("Menu Item #3", execute: (a, o) => Controller.Message("Menu Item #3 clicked!")));

            

            //Actions.Add(new SwitchThemeViewAction("Workplace", "Workplace (Office 2013) Theme", category: "View", categoryAccessKey: 'V', accessKey: 'W'));
            //Actions.Add(new SwitchThemeViewAction("Metro", "Metro Theme", category: "View", categoryAccessKey: 'V', accessKey: 'M'));
            //Actions.Add(new SwitchThemeViewAction("Battleship", "Windows 95 Theme", category: "View", categoryAccessKey: 'V', accessKey: 'W'));
            //Actions.Add(new SwitchThemeViewAction("Vapor", "Vapor Theme", category: "View", categoryAccessKey: 'V', accessKey: 'V'));
            //Actions.Add(new SwitchThemeViewAction("Geek", "Geek (Visual Studio) Theme", category: "View", categoryAccessKey: 'V', accessKey: 'G'));
            //Actions.Add(new SwitchThemeViewAction("Wildcat", "Wildcat Theme", category: "View", categoryAccessKey: 'V', accessKey: 'I'));
        }

        private void PlayWithSSRS()
        {
            //var rpt = new s
        }

        private void TestSharedProc()
        {
            var bob = SharedProcedures.DateToString(DateTime.Now, "Japan               ", true, "EN");
            
        }

        public void RunScheduledReports()
        {
            ServiceClient.Call<IPendingReportService>(service =>
            {
                var response = service.RunPendingReports(new RunPendingReportsRequest { RunNonThreaded = true, DevMode = false });

                if (!response.Success)
                {
                    Controller.Message("Error!: {0}", response.FailureInformation);
                }
            });

            Controller.Message("Done!");

        }

        public void RunBroadcasts()
        {
            ServiceClient.Call<IPendingReportService>(service =>
            {
                var response = service.RunPendingReports(new RunPendingReportsRequest { RunNonThreaded = true, DevMode = true });

                if (!response.Success)
                {
                    Controller.Message("Error!: {0}", response.FailureInformation);
                }
            });

            Controller.Message("Done!");

        }
     
        public void GenFilesFromCrystalReport()
        {
            try
            {
                var rptFile = "wtsFareSave";
                var outputPath = "C:\\CSCode\\Reports\\ReportFIles\\{0}";
                var crystalDir = "C:\\CSCode\\iBank.Net\\iBank.Services.Implementation\\CrystalReports\\{0}.rpt";
                //var outputPath = "C:\\iBank.Reports\\ReportFiles\\{0}";
                //var crystalDir = "C:\\iBank.Net\\iBank.NET\\iBank.Services.Implementation\\CrystalReports\\{0}.rpt";
                var rs = new ReportDocument();
                rs.Load(string.Format(crystalDir, rptFile));

                CrystalFunctions.GenXmlDataset(rs.Database.Tables[0], rs.Database.Tables[0].Name, string.Format(outputPath, rs.Database.Tables[0].Name));
                foreach (var subreport in rs.Subreports)
                {
                    var rpt = (ReportDocument) subreport;
                    CrystalFunctions.GenXmlDataset(rpt.Database.Tables[0], rpt.Name, string.Format(outputPath, rpt.Name));
                }
            }
            catch (Exception e)
            {
                Controller.Message(e.Message);
            }
        }
    }
}
