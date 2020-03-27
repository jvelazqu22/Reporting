using System;
using System.Windows.Forms;
using CODE.Framework.Services.Tools.Windows;

using iBank.BroadcastServer.Service;
using iBank.ReportServer;
using iBank.Services.Contracts;
using iBank.ReportServer.Service;

namespace Services.DevelopmentHost
{
    static class Program
    {
        /// <summary>
        /// This is a Windows Forms application designed to host your services during development.
        /// This application is not designed for use in production. 
        /// Host your services in a Windows Service project, in IIS, in Azure or in another Windows Forms application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var host = new TestServiceHost();

            host.AllowHttpCrossDomainCalls();

            //host.AddServiceHostBasicHttp(typeof(ReportService), true);
            //host.AddServiceHostWsHttp(typeof(ReportService), true);
            host.AddServiceHostNetTcp(typeof(ReportService), typeof(IOnDemandReportService));
            host.AddServiceHostNetTcp(typeof(BroadcastReportService), typeof(IPendingReportService));
            //host.AddServiceHostRestJson(typeof(ReportService));

            Application.Run(host);
        }
    }
}