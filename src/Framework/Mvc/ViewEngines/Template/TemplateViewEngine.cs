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

            this.AreaViewLocationFormats = new[]
              {
                // templates
                "~/Areas/{2}/Templates/{3}/Views/{1}/{0}.cshtml",
                //"~/Areas/{2}/Templates/{3}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Templates/{3}/Views/Shared/{0}.cshtml",
                //"~/Areas/{2}/Templates/{3}/Views/Shared/{0}.vbhtml",

                // default
                //"~/Areas/{2}/Views/{1}/{0}.cshtml",
                //"~/Areas/{2}/Views/{1}/{0}.vbhtml",
                //"~/Areas/{2}/Views/Shared/{0}.cshtml",
                //"~/Areas/{2}/Views/Shared/{0}.vbhtml"
              };
            this.AreaMasterLocationFormats = new[]
              {
                // templates
                "~/Areas/{2}/Templates/{3}/Views/{1}/{0}.cshtml",
                //"~/Areas/{2}/Templates/{3}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Templates/{3}/Views/Shared/{0}.cshtml",
                //"~/Areas/{2}/Templates/{3}/Views/Shared/{0}.vbhtml",

                // default
                //"~/Areas/{2}/Views/{1}/{0}.cshtml",
                //"~/Areas/{2}/Views/{1}/{0}.vbhtml",
                //"~/Areas/{2}/Views/Shared/{0}.cshtml",
                //"~/Areas/{2}/Views/Shared/{0}.vbhtml"
              };
            this.AreaPartialViewLocationFormats = new[]
              {
                // templates
                "~/Areas/{2}/Templates/{3}/Views/{1}/{0}.cshtml",
                //"~/Areas/{2}/Templates/{3}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Templates/{3}/Views/Shared/{0}.cshtml",
                //"~/Areas/{2}/Templates/{3}/Views/Shared/{0}.vbhtml",

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