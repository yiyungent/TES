﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class ErrorsController : Controller
    {
        public ViewResult WithoutAuth(string returnUrl)
        {
            ErrorRedirectViewModel model = new ErrorRedirectViewModel
            {
                Title = "权限不足",
                Message = "你没有权限访问此页面",
                RedirectUrl = Url.Action("Index", "Home", new { area = "", returnUrl = returnUrl }),
                RedirectUrlName = "首页",
                WaitSecond = 8
            };

            return View("_ErrorRedirect", model);
        }

        public ViewResult NeedLogin(string returnUrl)
        {
            ErrorRedirectViewModel model = new ErrorRedirectViewModel
            {
                Title = "请登录",
                Message = "该页面需要登录",
                RedirectUrl = Url.Action("Index", "Login", new { area = "Account", returnUrl = returnUrl }),
                RedirectUrlName = "登录页",
                WaitSecond = 8
            };

            return View("_ErrorRedirect", model);
        }

        public ViewResult LoginTimeOut(string returnUrl)
        {
            ErrorRedirectViewModel model = new ErrorRedirectViewModel
            {
                Title = "请重新登录",
                Message = "你的登录状态已超时",
                RedirectUrl = Url.Action("Index", "Login", new { area = "Account", returnUrl = returnUrl }),
                RedirectUrlName = "登录页",
                WaitSecond = 8
            };

            return View("_ErrorRedirect", model);
        }
    }
}
