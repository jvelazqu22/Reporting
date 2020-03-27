using System.Diagnostics;
using Domain.Models.WebModels;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using Microsoft.AspNetCore.Mvc;
using WebAdmin.Models;
using iBank.Repository.SQL.Repository;
using Microsoft.Extensions.Options;

namespace WebAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly IOptions<Connections> _connections;
        public HomeController(IOptions<AppSettings> settings, IOptions<Connections> connections)
        {
            _settings = settings;
            _connections = connections;
        }

        public IActionResult Index()
        {
            var dataTeste = new GetReportInputCriteriaByReportIdQuery(new MasterDataStore(_connections.Value.iBankMastersEntities).MastersQueryDb, "42-5068B9B9-ABA9-E67F-02A76ACFCB22F3B5_313_41641.keystonecf1").ExecuteQuery();

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
