﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Properties;
using System.Web.Routing;

namespace Framework.RequestResult
{
    /// <summary>
    /// 指示需要登录，当前登录超时
    /// <para>上次登录使用 "记住我", 当已超时</para>
    /// </summary>
    public class LoginTimeOutResultProvider
    {
        public static ActionResult Get(bool isAjax, string returnUrl = null)
        {
            ActionResult rtnResult = null;
            if (isAjax)
            {
                rtnResult = new Ajax_LoginTimeOutResult(returnUrl);
            }
            else
            {
                rtnResult = new LoginTimeOutResult(returnUrl);
            }

            return rtnResult;
        }

        public static ActionResult Get(HttpRequestBase request)
        {
            bool isAjax = request.IsAjaxRequest();
            string returnUrl = request.Url.AbsoluteUri;

            return Get(isAjax, returnUrl);
        }
    }

    /// <summary>
    /// 需要登录结果
    /// <para>修改自 <see cref="RedirectToRouteResult"/></para>
    /// </summary>
    public class LoginTimeOutResult : ActionResult
    {

        public LoginTimeOutResult(string returnUrl = null)
        {
            RouteValueDictionary routeValDic = new RouteValueDictionary();
            routeValDic.Add("controller", "Errors");
            routeValDic.Add("action", "LoginTimeOut");
            routeValDic.Add("area", "");
            if (!string.IsNullOrEmpty(returnUrl))
            {
                routeValDic.Add("returnUrl", returnUrl);
            }
            this.RouteValues = routeValDic;

            this.ReturnUrl = returnUrl;
        }

        /// <summary>
        /// 注意：在 new 时 RouteValues 其中的 returnUrl 就已经被决定
        /// </summary>
        public string ReturnUrl { get; private set; }

        public RouteValueDictionary RouteValues { get; set; }

        /// <summary>
        /// 因为 <see cref="MvcResources"/> 为 internal ，导致无法使用，暂时用此方法
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            RedirectToRouteResult redirectToRouteResult = new RedirectToRouteResult(this.RouteValues);
            redirectToRouteResult.ExecuteResult(context);
        }
    }

    public class Ajax_LoginTimeOutResult : JsonResult
    {
        public Ajax_LoginTimeOutResult(string returnUrl = null)
        {
            this.Data = new { code = -1, message = "登录超时，请重新登录", returnUrl = returnUrl };
            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            this.ReturnUrl = returnUrl;
        }

        /// <summary>
        /// 注意：在 new 时 Data 其中的 returnUrl 就已经被决定
        /// </summary>
        public string ReturnUrl { get; private set; }
    }
}
