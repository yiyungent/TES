using Framework.Attributes;
using Framework.Infrastructure.Concrete;
using Framework.Models;
using Framework.Mvc.ViewEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Framework.Config
{
    public static class FrameworkConfig
    {
        public static void Register()
        {
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterModelBinder(ModelBinders.Binders);
            RegisterViewEngine(ViewEngines.Engines);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LoginAccountFilterAttribute());
            filters.Add(new AuthFilterAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {

        }

        public static void RegisterModelBinder(ModelBinderDictionary binders)
        {
            binders.Add(typeof(CurrentAccountModel), new CurrentAccountModelBinder());
        }

        public static void RegisterViewEngine(ViewEngineCollection viewEngines)
        {
            viewEngines.Add(new GlobalSharedViewEngine());
        }
    }
}
