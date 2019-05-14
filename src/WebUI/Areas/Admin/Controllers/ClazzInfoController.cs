using Core;
using Domain;
using Framework.HtmlHelpers;
using Framework.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;
using WebUI.Areas.Admin.Models.Common;

namespace WebUI.Areas.Admin.Controllers
{
    public class ClazzInfoController : Controller
    {
        #region Ctor
        public ClazzInfoController()
        {
            ViewBag.PageHeader = "班级管理";
            ViewBag.PageHeaderDescription = "班级管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
            };
        }
        #endregion

        #region 列表
        public ActionResult Index(CurrentAccountModel currentAccount, int pageIndex = 1, int pageSize = 6)
        {
            ListViewModel<ClazzInfo> model = new ListViewModel<ClazzInfo>(pageIndex: pageIndex, pageSize: pageSize);
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(model);
        }
        #endregion




    }
}