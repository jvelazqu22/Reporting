using System;
using CODE.Framework.Services.Client;
using CODE.Framework.Wpf.Mvvm;
using iBank.Services.Contracts;
using iBank.ReportServer.Service;

namespace iBank.WPF.ReportTester.Models.Utilities
{

    public class RunOneReportViewModel : ViewModel
    {
        public string ReportId { get; set; }
        public ViewAction RunReport { get; set; }
        public ViewAction RunReportByProcessKey { get; set; }


        public RunOneReportViewModel()
        {
            RunReport = new ViewAction(execute:(o,a) => Go());
            RunReportByProcessKey = new ViewAction(execute: (o, a) => ByProcessKey());
        }

        private void Go()
        {
            var start = DateTime.Now;
            var reportService = new ReportService();
            var response = reportService.RunReportById(new RunReportByIdRequest { ReportId = ReportId, DevMode = true });

            if (!response.Success)
            {
                Controller.Message(string.Format("Error!: {0}", response.FailureInformation));
            }

            //ServiceClient.Call<IOnDemandReportService>(service =>
            //{
            //    var response = service.RunReportById(new RunReportByIdRequest { ReportId = ReportId, DevMode = true });

            //    if (!response.Success)
            //    {
            //        Controller.Message(string.Format("Error!: {0}", response.FailureInformation));
            //    }
            //});

            var end = DateTime.Now;

            Controller.Message(string.Format("Done! This report completed in {0} seconds.",(end - start).TotalSeconds));
        }

        public void ByProcessKey()
        {
            int key;
            if (!int.TryParse(ReportId, out key)) return;

            ServiceClient.Call<IOnDemandReportService>(service =>
            {
                var response = service.RunReportByProcessKey(new RunReportByProcessKeyRequest { ProcessKey = key });

                if (!response.Success)
                {
                    Controller.Message("Error!: {0}", response.FailureInformation);
                }
            });

            Controller.Message("Done!");

        }
    }
}
