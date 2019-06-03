using Core;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Framework.Mvc.ViewEngines.Templates
{
    public class DbDriveTemplateViewEngine : TemplateViewEngine
    {
        protected override string GetCurrentTemplate(ControllerContext controllerContext)
        {
            //var templateName = controllerContext.RequestContext.HttpContext.Request["Template"];
            //object templateNameSession = controllerContext.RequestContext.HttpContext.Session[TemplateSessionKey];
            string templateName = null;
            try
            {
                templateName = Container.Instance.Resolve<SettingService>().GetSet("DefaultTemplateName");
            }
            catch (Exception ex)
            { }

            return templateName;
        }
    }
}
