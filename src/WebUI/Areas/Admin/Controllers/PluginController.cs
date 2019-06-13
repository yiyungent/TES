using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using LightPlugin.Core;
using LightPlugin.Core.Domain.Cms;
using LightPlugin.Core.Plugins;
using LightPlugin.Services.Cms;
using WebUI.Areas.Admin.Models.PluginVM;
using WebUI.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    public class PluginController : Controller
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly IWebHelper _webHelper;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Constructors

        public PluginController()
        {
            //this._pluginFinder = pluginFinder;
            this._pluginFinder = new PluginFinder();
            //this._webHelper = webHelper;
            this._webHelper = new WebHelper(HttpContext);
            //this._widgetSettings = widgetSettings;
            this._widgetSettings = new WidgetSettings();
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual PluginModel PreparePluginModel(PluginDescriptor pluginDescriptor,
            bool prepareLocales = true, bool prepareStores = true)
        {
            var pluginModel = pluginDescriptor.ToModel();
            //logo
            pluginModel.LogoUrl = pluginDescriptor.GetLogoUrl(_webHelper);

            //configuration URLs

            if (pluginDescriptor.Installed)
            {
                //specify configuration URL only when a plugin is already installed

                //plugins do not provide a general URL for configuration
                //because some of them have some custom URLs for configuration
                //for example, discount requirement plugins require additional parameters and attached to a certain discount
                var pluginInstance = pluginDescriptor.Instance();
                string configurationUrl = null;
                if (pluginInstance is IWidgetPlugin)
                {
                    //Misc plugins
                    configurationUrl = Url.Action("ConfigureWidget", "Widget", new { systemName = pluginDescriptor.SystemName });
                }
                //else if (pluginInstance is IMiscPlugin)
                //{
                //    //Misc plugins
                //    configurationUrl = Url.Action("ConfigureMiscPlugin", "Plugin", new { systemName = pluginDescriptor.SystemName });
                //}
                pluginModel.ConfigurationUrl = configurationUrl;




                //enabled/disabled (only for some plugin types)
                if (pluginInstance is IWidgetPlugin)
                {
                    //Misc plugins
                    pluginModel.CanChangeEnabled = true;
                    pluginModel.IsEnabled = ((IWidgetPlugin)pluginInstance).IsWidgetActive(_widgetSettings);
                }

            }
            return pluginModel;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var viewModel = new PluginListViewModel();
            var allPluginDescriptor = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.All);
            foreach (var item in allPluginDescriptor)
            {
                viewModel.List.Add(item);
            }

            return View(viewModel);
        }

        public ActionResult Install(string systemName)
        {
            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //install plugin
                pluginDescriptor.Instance().Install();

                //restart application
                _webHelper.RestartAppDomain();
            }
            catch (Exception exc)
            {
            }

            return RedirectToAction("List");
        }
        public ActionResult Uninstall(string systemName)
        {
            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is installed
                if (!pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //uninstall plugin
                pluginDescriptor.Instance().Uninstall();

                //restart application
                _webHelper.RestartAppDomain();
            }
            catch (Exception exc)
            {
            }

            return RedirectToAction("List");
        }

        public ActionResult ReloadList()
        {
            //restart application
            _webHelper.RestartAppDomain();
            return RedirectToAction("List");
        }

        //edit
        public ActionResult EditPopup(string systemName)
        {
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                //No plugin found with the specified id
                return RedirectToAction("List");

            var model = PreparePluginModel(pluginDescriptor);

            return View(model);
        }
        [HttpPost]
        public ActionResult EditPopup(string btnId, string formId, PluginModel model)
        {
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(model.SystemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                //No plugin found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //we allow editing of 'friendly name', 'display order', store mappings
                pluginDescriptor.FriendlyName = model.FriendlyName;
                pluginDescriptor.DisplayOrder = model.DisplayOrder;
                pluginDescriptor.LimitedToStores.Clear();

                PluginFileParser.SavePluginDescriptionFile(pluginDescriptor);
                //reset plugin cache
                _pluginFinder.ReloadPlugins();
                //locales
                //foreach (var localized in model.Locales)
                //{
                //    pluginDescriptor.Instance().SaveLocalizedFriendlyName(_localizationService, localized.LanguageId, localized.FriendlyName);
                //}
                //enabled/disabled
                if (pluginDescriptor.Installed)
                {
                    var pluginInstance = pluginDescriptor.Instance();
                    if (pluginInstance is IWidgetPlugin)
                    {
                        //Misc plugins
                        var widget = (IWidgetPlugin)pluginInstance;
                        if (widget.IsWidgetActive(_widgetSettings))
                        {
                            if (!model.IsEnabled)
                            {
                                //mark as disabled
                                _widgetSettings.ActiveWidgetSystemNames.Remove(widget.PluginDescriptor.SystemName);
                                //_settingService.SaveSetting(_widgetSettings);
                            }
                        }
                        else
                        {
                            if (model.IsEnabled)
                            {
                                //mark as active
                                _widgetSettings.ActiveWidgetSystemNames.Add(widget.PluginDescriptor.SystemName);
                                //_settingService.SaveSetting(_widgetSettings);
                            }
                        }
                    }
                }

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion
    }
}