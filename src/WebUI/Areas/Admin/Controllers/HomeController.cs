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

namespace WebUI.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController()
        {

        }

        #region 后台主页
        public ViewResult Index(CurrentAccountModel currentAccount)
        {

            return View();
        }
        #endregion
    }
}