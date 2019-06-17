using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PluginHub;
using PluginHub.Infrastructure;
using PluginHub.Plugins;
using PluginHub.Services.Cms;
using PluginHub.Services.Configuration;
using PluginHub.Web.Mvc.Routes;

namespace BaiduTJ
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class BaiduTJPlugin : BasePlugin, IWidgetPlugin, IRouteProvider
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public int Priority { get { return 1; } }

        //public BaiduTJPlugin(ISettingService settingService,
        //     IWebHelper webHelper)
        //{
        //    this._settingService = settingService;
        //    this._webHelper = webHelper;
        //}

        public BaiduTJPlugin()
        {
            this._settingService = EngineContext.Current.Resolve<ISettingService>();
            this._webHelper = EngineContext.Current.Resolve<IWebHelper>();
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "home_page_bottom" };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsBaiduTJ";
            routeValues = new RouteValueDictionary { { "Namespaces", "PluginHub.Widgets.BaiduTJ.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsBaiduTJ";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "PluginHub.Widgets.BaiduTJ.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            BaiduTJSettings baiduTJSettings = new BaiduTJSettings();
            baiduTJSettings.TJCode = "Ĭ�ϴ���";
            _settingService.SaveSetting<BaiduTJSettings>(baiduTJSettings);

            //Open();

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {


            base.Uninstall();
        }

        public override void Open()
        {
            var route = RouteTable.Routes.MapRoute(
                   name: "Widgets.BaiduTJ",
                   url: "plugins/{controller}/{action}/{id}",
                   defaults: new { controller = "WidgetsBaiduTJ", action = "Configure", id = UrlParameter.Optional }
               );
            route.DataTokens["area"] = "plugins";

            base.Open();
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                   name: "Widgets.BaiduTJ",
                   url: "plugin-BaiduTJ/{controller}/{action}/{id}",
                   defaults: new { controller = "WidgetsBaiduTJ", action = "Configure", id = UrlParameter.Optional },
                   namespaces: new string[] { "BaiduTJ.Controllers" }
               );
        }
    }
}
