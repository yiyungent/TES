using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Properties;
using System.Web.Routing;

namespace Framework.RequestResult
{

    public class NeedLoginResultProvider
    {
        public static ActionResult Get(bool isAjax, string returnUrl = null)
        {
            ActionResult rtnResult = null;
            if (isAjax)
            {
                rtnResult = new Ajax_NeedLoginResult(returnUrl);
            }
            else
            {
                rtnResult = new NeedLoginResult(returnUrl);
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
    public class NeedLoginResult : ActionResult
    {

        public NeedLoginResult(string returnUrl = null)
        {
            RouteValueDictionary routeValDic = new RouteValueDictionary();
            routeValDic.Add("controller", "Errors");
            routeValDic.Add("action", "NeedLogin");
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

    public class Ajax_NeedLoginResult : JsonResult
    {
        public Ajax_NeedLoginResult(string returnUrl = null)
        {
            this.Data = new { code = -1, message = "请登录", returnUrl = returnUrl };
            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            this.ReturnUrl = returnUrl;
        }

        /// <summary>
        /// 注意：在 new 时 Data 其中的 returnUrl 就已经被决定
        /// </summary>
        public string ReturnUrl { get; private set; }
    }
}
