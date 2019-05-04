using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Areas.Account.Controllers
{
    public class ProfileController : Controller
    {
        #region 个人中心首页
        public ActionResult Index()
        {
            return View();
        } 
        #endregion
    }
}