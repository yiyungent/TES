using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;
using Domain;
using Framework.Common;
using Service;
using Framework.Config;
using WebUI.Areas.Account.Models;
using NHibernate.Criterion;
using Framework.Infrastructure.Concrete;

namespace WebUI.Areas.Account.Controllers
{
    public class LoginController : Controller
    {
        private static string _sessionKeyLoginAccount = AppConfig.LoginAccountSessionKey;
        private static string _cookieKeyToken = AppConfig.RememberMeTokenCookieKey;
        private static int _rememberMeDayCount = AppConfig.RememberMeDayCount;

        #region 登录
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            #region 检查 登录状态
            if (Session[_sessionKeyLoginAccount] != null)
            {
                return LoginSuccessRedirectResult(returnUrl);
            }
            #region 记住我
            if (Request.Cookies.AllKeys.Contains(_cookieKeyToken))
            {
                if (Request.Cookies[_cookieKeyToken] != null && string.IsNullOrEmpty(Request.Cookies[_cookieKeyToken].Value) == false)
                {
                    string cookieTokenValue = Request.Cookies[_cookieKeyToken].Value;
                    UserInfo user = Container.Instance.Resolve<UserInfoService>().Query(new List<ICriterion>
                    {
                        Expression.Eq(_cookieKeyToken, cookieTokenValue)
                    }).FirstOrDefault();

                    if (user == null)
                    {
                        // 口令不正确
                        Response.Cookies[_cookieKeyToken].Expires = DateTime.UtcNow.AddDays(-1);
                    }
                    else if (user.LastLoginTime.AddDays(_rememberMeDayCount) > DateTime.UtcNow)
                    {
                        // 最多 "记住我" 保存7天的 登录状态
                        Session[_sessionKeyLoginAccount] = user;
                        return LoginSuccessRedirectResult(returnUrl);
                    }
                    else
                    {
                        // 登录 已过期
                        ModelState.AddModelError("", "登录已过期，请重新登录");
                    }
                }
            }
            #endregion

            #endregion

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model, string returnUrl)
        {
            #region 验证码验证
            if (Session["ValidateCode"] != null && model.ValidateCode != null && model.ValidateCode.ToLower() != Session["ValidateCode"].ToString())
            {
                ModelState.AddModelError("Error_PersonLogin", "验证码错误！");
                return View();
            }
            Session["ValidateCode"] = null;
            #endregion

            if (ModelState.IsValid)
            {
                var dbUser = Container.Instance.Resolve<UserInfoService>().Query(new List<ICriterion>
                {
                    Expression.Eq("LoginAccount", model.LoginAccount)
                }).FirstOrDefault();
                if (dbUser == null)
                {
                    ModelState.AddModelError("", "账号不存在");
                    return View();
                }
                if (dbUser.Password != EncryptHelper.MD5Encrypt32(model.Password))
                {
                    ModelState.AddModelError("", "账号或密码错误");
                    return View();
                }
                if (dbUser.Status == 1)
                {
                    ModelState.AddModelError("", "账号被禁用");
                    return View();
                }

                // 登录成功
                Session[_sessionKeyLoginAccount] = dbUser;
                // 浏览器移除 Token
                if (Request.Cookies.AllKeys.Contains(_cookieKeyToken))
                {
                    Response.Cookies[_cookieKeyToken].Expires = DateTime.UtcNow.AddDays(-1);
                }

                #region 记住我
                if (model.IsRememberMe == true)
                {
                    string token = Guid.NewGuid().ToString();
                    // token 存入登录用户--数据库
                    dbUser.Token = token;
                    // token 存入 浏览器
                    HttpCookie cookieToken = new HttpCookie(_cookieKeyToken, token)
                    {
                        Expires = DateTime.UtcNow.AddDays(_rememberMeDayCount),
                        HttpOnly = true
                    };
                    Response.Cookies.Add(cookieToken);
                }
                else
                {
                    // 数据库-当前用户移除 Token
                    dbUser.Token = null;
                }
                #endregion

                // 更新用户--最后登录时间等
                dbUser.LastLoginTime = DateTime.UtcNow;
                Container.Instance.Resolve<UserInfoService>().Edit(dbUser);

                return LoginSuccessRedirectResult(returnUrl);
            }

            return View(model);
        }
        #endregion

        #region 登录成功跳转结果
        private ActionResult LoginSuccessRedirectResult(string returnUrl)
        {
            if (Request.IsAjaxRequest())
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return Json(new { code = 1, message = "登录成功", targetUrl = Url.Action("Index", "Home", new { area = "" }) });
                }
                else
                {
                    return Json(new { code = 1, message = "登录成功", targetUrl = returnUrl });
                }
            }
            else
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
        }
        #endregion

        #region 退出账号
        public ViewResult Exit(string returnUrl = null)
        {
            if (returnUrl == null)
            {
                returnUrl = Url.Action("Index", "Home", new { area = "" });
            }
            ViewBag.ReturnUrl = returnUrl;
            AccountManager.Exit();

            return View();
        }
        #endregion
    }
}
