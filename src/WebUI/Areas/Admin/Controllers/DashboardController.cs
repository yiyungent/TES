using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Areas.Admin.Controllers
{
    /// <summary>
    /// 仪表盘
    /// </summary>
    public class DashboardController : Controller
    {
        public ActionResult One()
        {
            return View();
        }
    }
}