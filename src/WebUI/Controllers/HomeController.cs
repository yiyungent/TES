﻿using Domain;
using Framework.Infrastructure.Concrete;
using Framework.Models;
using Framework.Mvc;
using Framework.Mvc.ViewEngine.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Controllers
{
    public class HomeController : BaseController
    {
        #region Ctor
        public HomeController()
        {

        }
        #endregion

        public ActionResult Index()
        {
            // 当前登录账号/未登录
            CurrentAccountModel currentAccount = AccountManager.GetCurrentAccount();
            ViewBag.CurrentAccount = currentAccount;
            Session[ThemeViewEngine.ThemeSessionKey] = "Red";

            //return View(currentAccount);
            return View("Index", "_TestMaster", currentAccount);
            //return PartialView("_TopNav_UserAccountMenuPartial");
        }
    }
}