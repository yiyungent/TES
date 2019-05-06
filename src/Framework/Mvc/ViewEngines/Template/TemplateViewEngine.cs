using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Framework.Mvc.ViewEngines.Template
{
    public class TemplateViewEngine : TemplateBuildManagerViewEngine
    {
        // mod 修改视图检索路径
        public TemplateViewEngine()
        {
            // {0} ViewName
            // {1} ControllerName
            // {2} AreaName
            // {3} TemplateName

            // 注意：当 Area 区域的 找不到 视图时，默认会 到外层 Views 寻找视图
            // 所以：在设计 TemplateViewEngine 时，为避免 当某 Area/Admin/Views 区域内 无目标模板视图时，导致出现向外搜索到不合理的视图，所以 不将 Templates 文件夹放入 ~/Areas/ 中，
            // 而是完全所有模板视图文件均在 ~/Templates
            // eg. 若有 Areas/Admin, 主题模板为 "Red", 则主题模板视图在 ~/Templates/Red/Areas/Admin
            // 若找不到目标主题模板视图，则放弃，让下一个视图引擎进行搜索

            this.AreaViewLocationFormats = new[]
              {
                // templates
                "~/Templates/{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                //"~/Templates/{3}/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Templates/{3}/Areas/{2}/Views/Shared/{0}.cshtml",
                //"~/Templates/{3}/Areas/{2}/Views/Shared/{0}.vbhtml",

                // default
                //"~/Areas/{2}/Views/{1}/{0}.cshtml",
                //"~/Areas/{2}/Views/{1}/{0}.vbhtml",
                //"~/Areas/{2}/Views/Shared/{0}.cshtml",
                //"~/Areas/{2}/Views/Shared/{0}.vbhtml"
              };
            this.AreaMasterLocationFormats = new[]
              {
                // templates
                "~/Templates/{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                //"~/Templates/{3}/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Templates/{3}/Areas/{2}/Views/Shared/{0}.cshtml",
                //"~/Templates/{3}/Areas/{2}/Views/Shared/{0}.vbhtml",

                // default
                //"~/Areas/{2}/Views/{1}/{0}.cshtml",
                //"~/Areas/{2}/Views/{1}/{0}.vbhtml",
                //"~/Areas/{2}/Views/Shared/{0}.cshtml",
                //"~/Areas/{2}/Views/Shared/{0}.vbhtml"
              };
            this.AreaPartialViewLocationFormats = new[]
              {
                // templates
                "~/Templates/{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                //"~/Templates/{3}/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Templates/{3}/Areas/{2}/Views/Shared/{0}.cshtml",
                //"~/Templates/{3}/Areas/{2}/Views/Shared/{0}.vbhtml",

                // default
                //"~/Areas/{2}/Views/{1}/{0}.cshtml",
                //"~/Areas/{2}/Views/{1}/{0}.vbhtml",
                //"~/Areas/{2}/Views/Shared/{0}.cshtml",
                //"~/Areas/{2}/Views/Shared/{0}.vbhtml"
              };
            this.ViewLocationFormats = new[]
              {
                // templates
                "~/Templates/{2}/Views/{1}/{0}.cshtml",
                //"~/Templates/{2}/Views/{1}/{0}.vbhtml",
                "~/Templates/{2}/Views/Shared/{0}.cshtml",
                //"~/Templates/{2}/Views/Shared/{0}.vbhtml",

                // default
                //"~/Views/{1}/{0}.cshtml",
                //"~/Views/{1}/{0}.vbhtml",
                //"~/Views/Shared/{0}.cshtml",
                //"~/Views/Shared/{0}.vbhtml"
              };
            this.MasterLocationFormats = new[]
              {
                // templates
                "~/Templates/{2}/Views/{1}/{0}.cshtml",
                //"~/Templates/{2}/Views/{1}/{0}.vbhtml",
                "~/Templates/{2}/Views/Shared/{0}.cshtml",
                //"~/Templates/{2}/Views/Shared/{0}.vbhtml",

                // default
                //"~/Views/{1}/{0}.cshtml",
                //"~/Views/{1}/{0}.vbhtml",
                //"~/Views/Shared/{0}.cshtml",
                //"~/Views/Shared/{0}.vbhtml"
              };
            this.PartialViewLocationFormats = new[]
              {
                // templates
                "~/Templates/{2}/Views/{1}/{0}.cshtml",
                //"~/Templates/{2}/Views/{1}/{0}.vbhtml",
                "~/Templates/{2}/Views/Shared/{0}.cshtml",
                //"~/Templates/{2}/Views/Shared/{0}.vbhtml",

                // default
                //"~/Views/{1}/{0}.cshtml",
                //"~/Views/{1}/{0}.vbhtml",
                //"~/Views/Shared/{0}.cshtml",
                //"~/Views/Shared/{0}.vbhtml"
              };
            this.FileExtensions = new[]
            {
            "cshtml",
            //"vbhtml"
            };


        }

        #region mod 重写 RazorViewEngine 中

        // 去掉 base.ViewPageActivator， DisplayModeProvider 的设置

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            string layoutPath = null;
            var runViewStartPages = false;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions);
            //return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions, base.ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            string layoutPath = masterPath;
            var runViewStartPages = true;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, viewPath, layoutPath, runViewStartPages, fileExtensions);
        }

        #endregion

    }
}