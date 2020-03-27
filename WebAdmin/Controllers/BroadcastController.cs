using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.WebModels;
using Domain.Models.WebModels.BroadcastDiagnostics;
using Domain.Orm.iBankMastersQueries.BroadcastQueries.Diagnostics;
using iBank.Repository.SQL.Repository;
using Microsoft.AspNetCore.Mvc;
using WebAdmin.Models;
using Microsoft.Extensions.Options;

namespace WebAdmin.Controllers
{
    public class BroadcastController : Controller
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly IOptions<Connections> _connections;
        public BroadcastController(IOptions<AppSettings> settings, IOptions<Connections> connections)
        {
            _settings = settings;
            _connections = connections;

        }

        public IActionResult Index()
        {
            //var dataTeste = new GetReportInputCriteriaByReportIdQuery(new MasterDataStore(_connections.Value.iBankMastersEntities).MastersQueryDb, "42-5068B9B9-ABA9-E67F-02A76ACFCB22F3B5_313_41641.keystonecf1").ExecuteQuery();
            var taskList = new List<Task>();
            var diagnostics = new DiagnosticsParams();

            taskList.Add(Task.Factory.StartNew(() =>
            {
                diagnostics.MainDiagnostics = new GetMainDiagnosticsQuery(new MasterDataStore(_connections.Value.iBankMastersEntities).MastersQueryDb).ExecuteQuery();
            }));

            taskList.Add(Task.Factory.StartNew(() =>
            {
                diagnostics.ReportingErrorLogs = new GetLast24HrsErrorLogsQuery(new MasterDataStore(_connections.Value.iBankMastersEntities).MastersQueryDb)
                    .ExecuteQuery()
                    .ToList();
            }));

            Task.WaitAll(taskList.ToArray());
            diagnostics.MapDataModels();

            return View(diagnostics);
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
