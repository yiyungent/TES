using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Admin.Controllers
{
    public class EvaTaskController : Controller
    {
        #region Ctor
        public EvaTaskController()
        {
            ViewBag.PageHeader = "评价任务管理";
            ViewBag.PageHeaderDescription = "评价管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
            };
        }
        #endregion

        #region 列表
        public ActionResult Index()
        {
            return View();
        } 
        #endregion
    }
}