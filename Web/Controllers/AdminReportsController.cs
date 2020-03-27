using Domain.Constants;
using Domain.Orm.iBankAdminQueries;

using PagedList;
using System.Linq;
using System.Web.Mvc;

using Domain.Models.ViewModels;
using Domain.Orm.WebQueries;

using iBank.Repository.SQL.Repository;

namespace Web.Controllers
{
    public class AdminReportsController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BroadcastOverview()
        {
            return View();
        }

        public ActionResult BroadcastStatus()
        {
            return View();
        }

        public ActionResult CompanyUsers(int? page = 1)
        {
            int pageNumber = (page ?? 1);
            int pageSize = Size.NUMBER_OF_RECORDS_TO_DISPLAY_PER_WEB_PAGE;
            var users = new GetAllUsersQuery(new iBankClientQueryable(WebConfigurations.SQL_SERVER_NAME, WebConfigurations.SQL_DATABASE_NAME)).ExecuteQuery();
            var userList = users.ToList();
            return View(userList.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CustomLayout()
        {
            return View();
        }

        public ActionResult DocumentDeliveryLog()
        {
            return View();
        }

        public ActionResult iBankEventLog()
        {
            return View();
        }

        private void UpdateSearchViewBagValues(SearchLoadLogViewModel searchModel)
        {
            ViewBag.LoadTypeSelected = searchModel.LoadTypeSelected;
            ViewBag.FromLoadDateSelected = searchModel.FromLoadDateSelected;
            ViewBag.ToLoadDateSelected = searchModel.ToLoadDateSelected;
            ViewBag.GdsBo = searchModel.GdsBo;
            ViewBag.DataSource = searchModel.DataSource;
        }

        public ActionResult Log(SearchLoadLogViewModel searchModel, int? page = 1)
        {
            int pageNumber = (page ?? 1);
            int pageSize = Size.NUMBER_OF_RECORDS_TO_DISPLAY_IN_LOAD_LOG_PAGE;
            var logs = new GetAllLoadLogQuery(new iBankMastersQueryable(), searchModel).ExecuteQuery();

            UpdateSearchViewBagValues(searchModel);
            return View(logs.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Log(SearchLoadLogViewModel searchModel)
        {
            int pageNumber = 1;
            int pageSize = Size.NUMBER_OF_RECORDS_TO_DISPLAY_IN_LOAD_LOG_PAGE;
            var logs = new GetAllLoadLogQuery(new iBankMastersQueryable(), searchModel).ExecuteQuery();

            UpdateSearchViewBagValues(searchModel);
            return View(logs.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SavedFiltersList()
        {
            return View();
        }
    }
}