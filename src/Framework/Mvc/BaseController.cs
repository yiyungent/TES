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

namespace Framework.Mvc
{
    public class BaseController : Controller
    {
        protected virtual IAuthManager AuthManager { get; set; }

        protected virtual CurrentAccountModel CurrentAccount { get; set; }

        public BaseController()
        {
            this.AuthManager = AuthManagerFactory.Get();
            this.CurrentAccount = AccountManager.GetCurrentAccount();

            UserInfo currentUserInfo = AccountManager.GetCurrentUserInfo();
            ViewBag.CurrentUserInfo = currentUserInfo;
            ViewBag.MenuList = this.AuthManager.GetMenuListByUserInfo(currentUserInfo);
        }
    }
}
