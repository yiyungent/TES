using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Core;
using Domain;
using Framework.Config;
using Framework.Common;
using WebUI.Controllers;

namespace WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private Container _container;

        protected void Application_Start()
        {
            #region 容器初始化
            try
            {
                // 获取 web.config 配置
                IConfigurationSource source = (IConfigurationSource)System.Configuration.ConfigurationManager.GetSection("ActiveRecord");
                ActiveRecordStarter.Initialize(typeof(UserInfo).Assembly, source);
                _container = Container.Instance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configure(WebApiConfig.Register);

            FrameworkConfig.Register();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            //process 404 HTTP errors
            var httpException = exception as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404)
            {
                var webHelper = new WebHelper(new HttpContextWrapper(Context));
                if (!webHelper.IsStaticResource(this.Request))
                {
                    Response.Clear();
                    Server.ClearError();
                    Response.TrySkipIisCustomErrors = true;

                    // Call target Controller and pass the routeData.
                    IController errorController = new ErrorsController();

                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "Errors");
                    routeData.Values.Add("action", "PageNotFound");

                    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                }
            }
        }
    }
}
