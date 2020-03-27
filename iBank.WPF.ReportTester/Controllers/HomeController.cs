using CODE.Framework.Wpf.Mvvm;
using iBank.WPF.ReportTester.Models.Home;

namespace iBank.WPF.ReportTester.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Start()
        {
            return Shell(new StartViewModel(), "Business Application");
        }
    }
}
