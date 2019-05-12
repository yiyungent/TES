using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Attributes
{
    using Core;
    using Domain;
    using Framework.Common;
    using Framework.Config;
    using Service;
    using NHibernate.Criterion;
    using System.Web.Mvc;
    using Framework.Infrastructure.Abstract;
    using Framework.Factories;

    /// <summary>
    /// 登录用户Session 维护器
    /// <para>当浏览器端Session被移除，但拥有"记住我"Cookie时，只要有效，则会在任何请求之前 将此用户信息存于 Session</para>
    /// </summary>
    public class LoginAccountFilterAttribute : ActionFilterAttribute
    {
        private static string _sessionKeyLoginAccount = AppConfig.LoginAccountSessionKey;
        private static string _cookieKeyToken = AppConfig.RememberMeTokenCookieKey;
        private static int _rememberMeDayCount = AppConfig.RememberMeDayCount;

        private static IDBAccessProvider _dBAccessProvider = HttpOneRequestFactory.Get<IDBAccessProvider>();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string areaName = filterContext.RouteData.DataTokens["area"] == null ? "" : filterContext.RouteData.DataTokens["area"].ToString();
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            // hack install url
            if (areaName == "" && controllerName == "Install")
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            this.CheckRememberMe(filterContext);
            base.OnActionExecuting(filterContext);
        }

        #region 维护当前请求 Session 始终有登录用户值（若 已登录）
        /// <summary>
        /// 检查当前请求是否 为"记住我"(记住登录状态)，如果是，则为其Session再次赋值此用户
        /// </summary>
        private void CheckRememberMe(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;
            var session = filterContext.HttpContext.Session;
            // 如果 Session 中已经有 登录用户，则不需要再根据 "记住我" Token 保存登录用户到 Session
            UserInfo userInfo = Tools.GetSession<UserInfo>(AppConfig.LoginAccountSessionKey);
            if (userInfo != null)
            {
                return;
            }
            // 是否请求浏览器有 "记住我" Token
            if (request.Cookies.AllKeys.Contains(_cookieKeyToken))
            {
                if (request.Cookies[_cookieKeyToken] != null && string.IsNullOrEmpty(request.Cookies[_cookieKeyToken].Value) == false)
                {
                    string cookieTokenValue = request.Cookies[_cookieKeyToken].Value;
                    UserInfo user = _dBAccessProvider.GetUserInfoByTokenCookieKey(cookieTokenValue);

                    if (user == null)
                    {
                        // 口令不正确
                        response.Cookies[_cookieKeyToken].Expires = DateTime.UtcNow.AddDays(-1);
                    }
                    else if (user.LastLoginTime.AddDays(_rememberMeDayCount) > DateTime.UtcNow)
                    {
                        // 最多 "记住我" 保存7天的 登录状态
                        // 保存到 Session
                        session[_sessionKeyLoginAccount] = user;
                    }
                    else
                    {
                        // 登录 已过期
                    }
                }
            }
        }
        #endregion
    }
}
