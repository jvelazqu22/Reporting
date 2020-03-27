using CODE.Framework.Wpf.Mvvm;
using iBank.WPF.ReportTester.Models.Utilities;

namespace iBank.WPF.ReportTester.Controllers
{
    public class UtilitiesController : Controller
    {
        public ActionResult RunOneReport()
        {
            return View(new RunOneReportViewModel());
        }

        public ActionResult GenPOCO()
        {
            return View(new GenPOCOViewModel());
        }
    }
}
