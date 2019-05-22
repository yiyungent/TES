using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Admin.Controllers
{
    public class DepartmentController : Controller
    {
        #region Ctor
        public DepartmentController()
        {
            ViewBag.PageHeader = "部门管理";
            ViewBag.PageHeaderDescription = "部门管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
            };
        }
        #endregion

        public ActionResult Index()
        {
            return View();
        }
    }
}