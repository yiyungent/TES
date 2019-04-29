using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class ErrorsController : Controller
    {
        public ViewResult WithoutAuth()
        {
            return View();
        }

        public ViewResult NeedLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }
    }
}
