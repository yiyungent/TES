﻿using System.Web.Routing;
//using Nop.Web.Framework.Mvc;

namespace WebUI.Models.Cms
{
    //public partial class RenderWidgetModel : BaseNopModel
    public partial class RenderWidgetModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}