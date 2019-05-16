using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Admin.Controllers
{
    public class SysMenuController : Controller
    {
        #region Ctor
        public SysMenuController()
        {
            ViewBag.PageHeader = "菜单管理";
            ViewBag.PageHeaderDescription = "菜单管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("系统管理"),
            };
        }
        #endregion

        public ActionResult Index()
        {
            return View();
        }
    }
}