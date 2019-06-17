using PluginHub.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models.PluginVM
{
    public class PluginListViewModel
    {
        public IList<PluginDescriptor> List { get; set; }

        #region Ctor
        public PluginListViewModel()
        {
            this.List = new List<PluginDescriptor>();
        }
        #endregion
    }
}