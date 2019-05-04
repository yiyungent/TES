using Domain;
using Framework.Infrastructure.Concrete;
using Framework.Models;
using Framework.Mvc;
using Framework.RequestResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Account.Controllers
{
    public class ProfileController : BaseController
    {
        #region Ctor
        public ProfileController()
        {
            ViewBag.PageHeader = "个人中心";
            ViewBag.PageHeaderDescription = "";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("主页"),
            };
        }
        #endregion

        #region 个人中心首页
        public ActionResult Index(string loginAccount = null)
        {
            if (loginAccount == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            // 此主页对应的UserInfo
            UserInfo model = AccountManager.GetUserInfoByLoginAccount(loginAccount);
            if (model == null)
            {
                // 不存在此用户
                return new View_NotExistAccountResult();
            }

            return View(model);
        }
        #endregion
    }
}