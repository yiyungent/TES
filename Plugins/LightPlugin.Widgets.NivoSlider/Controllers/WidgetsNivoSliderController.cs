using System.Web.Mvc;
using LightPlugin.Core;
using LightPlugin.Core.Caching;
//using LightPlugin.Plugin.Widgets.NivoSlider.Infrastructure.Cache;
//using LightPlugin.Plugin.Widgets.NivoSlider.Models;
//using LightPlugin.Services.Configuration;
//using LightPlugin.Services.Localization;
//using LightPlugin.Services.Media;
//using LightPlugin.Services.Stores;
//using LightPlugin.Web.Framework.Controllers;

namespace LightPlugin.Widgets.NivoSlider.Controllers
{
    public class WidgetsNivoSliderController : System.Web.Mvc.Controller
    {

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            

            return View("~/Plugins/Widgets.NivoSlider/Views/WidgetsNivoSlider/PublicInfo.cshtml");
        }

        public ContentResult Test()
        {
            return Content("测试test");
        }

    }
}