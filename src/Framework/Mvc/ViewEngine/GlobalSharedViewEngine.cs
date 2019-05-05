using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Framework.Mvc.ViewEngine
{
    public class GlobalSharedViewEngine : RazorViewEngine
    {
        public GlobalSharedViewEngine()
            : this((IViewPageActivator)null)
        {
        }

        public GlobalSharedViewEngine(IViewPageActivator viewPageActivator)
           : base(viewPageActivator)
        {
            this.AreaViewLocationFormats = new[]
            {
                "~/GlobalShared/Views/{0}.cshtml"
            };
            this.AreaMasterLocationFormats = new[]
            {
                "~/GlobalShared/Views/{0}.cshtml"
            };
            this.AreaPartialViewLocationFormats = new[]
            {
                "~/GlobalShared/Views/{0}.cshtml"
            };
            this.ViewLocationFormats = new[]
            {
                "~/GlobalShared/Views/{0}.cshtml"
            };
            this.MasterLocationFormats = new[]
            {
                "~/GlobalShared/Views/{0}.cshtml"
            };
            this.PartialViewLocationFormats = new[]
            {
                "~/GlobalShared/Views/{0}.cshtml"
            };
            this.FileExtensions = new[]
            {
                "cshtml"
            };
        }
    }
}
