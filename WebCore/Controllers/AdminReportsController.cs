using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Domain.Constants;
using iBank.Services.Orm.iBankAdminQueries;
using iBank.Services.Orm.Databases;
using PagedList;
using iBank.Services.Orm.WebQueries;
using iBank.Services.Orm;
using Domain.WebModels;
using Microsoft.Extensions.Options;

namespace WebCore.Controllers
{
    public class AdminReportsController : Controller
    {
        AppSettings _appSettings;
        public AdminReportsController(IOptions<AppSettings> settings)
        {
            _appSettings = settings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BroadcastOverview()
        {
            return View();
        }

        public IActionResult BroadcastStatus()
        {
            return View();
        }

        public IActionResult CompanyUsers(int? page = 1)
        {
            int pageNumber = (page ?? 1);
            int pageSize = Size.NUMBER_OF_RECORDS_TO_DISPLAY_PER_WEB_PAGE;
            var users = new GetAllUsersQuery(new iBankClientQueryable(_appSettings.SqlServerName, _appSettings.SqlServerName)).ExecuteQuery();
            var userList = users.ToList();
            return View(userList.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult CustomLayout()
        {
            return View();
        }

        public IActionResult DocumentDeliveryLog()
        {
            return View();
        }

        public IActionResult iBankEventLog()
        {
            return View();
        }

        public IActionResult Log(int? page = 1)
        {
            int pageNumber = (page ?? 1);
            int pageSize = Size.NUMBER_OF_RECORDS_TO_DISPLAY_IN_LOAD_LOG_PAGE;
            var logs = new GetAllLoadLogQuery(new iBankMastersEntities()).ExecuteQuery();
            return View(logs.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult SavedFiltersList()
        {
            return View();
        }

    }
}