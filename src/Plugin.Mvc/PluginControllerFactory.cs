﻿
using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;

namespace Plugin.Mvc
{
    /// <summary>
    /// 插件控制器工厂。
    /// </summary>
    public class PluginControllerFactory : DefaultControllerFactory
    {
        /// <summary>
        /// 根据控制器名称及请求信息获得控制器类型。
        /// </summary>
        /// <param name="requestContext">请求信息</param>
        /// <param name="controllerName">控制器名称。</param>
        /// <returns>控制器类型。</returns>
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            string pluginName = string.Empty;
            Type controllerType = null;

            if (requestContext.RouteData.Values.ContainsKey("pluginName"))
            {
                pluginName = requestContext.RouteData.GetRequiredString("pluginName");
                controllerType = this.GetControllerType(pluginName, controllerName);
            }

            if (controllerType == null)
            {
                controllerType = base.GetControllerType(requestContext, controllerName);
            }

            return controllerType;
        }

        /// <summary>
        /// 根据控制器名称获得控制器类型。
        /// </summary>
        /// <param name="controllerName">控制器名称。</param>
        /// <returns>控制器类型。</returns>
        private Type GetControllerType(string pluginName, string controllerName)
        {
            var plugin = Plugin.Framework.PluginManager.GetPlugin(pluginName);
            var controlName = controllerName + "Controller";
            //var control = plugin.Assembly.GetTypes().FirstOrDefault(p => p.Name == controlName); ;
            // 注意：有时 controllerName 首字母小写，所以进行忽略大小写比较，不然查不到
            var control = plugin.Assembly.GetTypes().FirstOrDefault(p => p.Name.ToLower() == controlName.ToLower());
            if (control != null)
            {
                return control;
            }

            return null;
        }
    }
}