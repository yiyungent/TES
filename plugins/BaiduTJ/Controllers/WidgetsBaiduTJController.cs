﻿using PluginHub.Services.Configuration;
using BaiduTJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PluginHub.Infrastructure;

namespace BaiduTJ.Controllers
{
    public class WidgetsBaiduTJController : Controller
    {
        private readonly ISettingService _settingService;

        //public WidgetsBaiduTJController(ISettingService settingService)
        //{
        //    this._settingService = settingService;
        //}

        public WidgetsBaiduTJController()
        {
            this._settingService = EngineContext.Current.Resolve<ISettingService>();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            BaiduTJSettings baiduTJSettings = _settingService.LoadSetting<BaiduTJSettings>();
            ViewBag.TJCode = baiduTJSettings.TJCode;

            return View("~/Plugins/BaiduTJ/Views/WidgetsBaiduTJ/PublicInfo.cshtml");
        }

        [HttpGet]
        //[ChildActionOnly]
        public ActionResult Configure()
        {
            BaiduTJSettings baiduTJSettings = _settingService.LoadSetting<BaiduTJSettings>();

            ConfigurationViewModel viewModel = new ConfigurationViewModel();
            viewModel.TJCode = baiduTJSettings.TJCode;

            return View("~/Plugins/BaiduTJ/Views/WidgetsBaiduTJ/Configure.cshtml", viewModel);
        }

        [HttpPost]
        //[ChildActionOnly]
        public ActionResult Configure(ConfigurationViewModel inputModel)
        {
            BaiduTJSettings baiduTJSettings = _settingService.LoadSetting<BaiduTJSettings>();
            baiduTJSettings.TJCode = inputModel.TJCode;
            try
            {
                _settingService.SaveSetting<BaiduTJSettings>(baiduTJSettings);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("BaiduTJ.Configure", "保存失败");
            }

            return View("~/Plugins/BaiduTJ/Views/WidgetsBaiduTJ/Configure.cshtml");
        }
    }
}