using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;
using Service;
using Framework.Infrastructure.Abstract;
using Framework.Infrastructure.Concrete;
using Framework.Models;
using Framework.Factories;
using Framework.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController()
        {
            ViewBag.PageHeader = "TES";
            ViewBag.PageHeaderDescription = "后台概述";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>();
        }

        #region 后台框架
        public ViewResult Index(CurrentAccountModel currentAccount)
        {
            return View();
        }
        #endregion

        #region 后台概述
        public ViewResult Default()
        {
            return View();
        }
        #endregion
    }
}