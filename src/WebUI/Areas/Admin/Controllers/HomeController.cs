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

namespace WebUI.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        IAuthManager _authManager = AuthManagerFactory.Get();

        #region 后台主页
        public ViewResult Index(CurrentAccountModel currentAccount)
        {
            ViewBag.LoginAccount = currentAccount.UserInfo;

            ViewBag.MenuList = _authManager.GetMenuListByAccount(currentAccount.UserInfo);

            return View();
        }
        #endregion
    }
}