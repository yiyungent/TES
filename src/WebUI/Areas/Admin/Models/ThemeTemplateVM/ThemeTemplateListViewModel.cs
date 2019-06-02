using Framework.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models.ThemeTemplateVM
{
    public class ThemeTemplateViewModel
    {
        public IList<dynamic> List { get; set; }

        public PageInfo PageInfo { get; set; }
    }

    public enum Source
    {
        /// <summary>
        /// 本地上传
        /// </summary>
        Upload,

        /// <summary>
        /// 应用市场
        /// </summary>
        AppStore,
    }

    public static class SourceExt
    {
        public static string GetStr(this Source source)
        {
            string rtn = string.Empty;
            switch (source)
            {
                case Source.Upload:
                    rtn = "本地上传";
                    break;
                case Source.AppStore:
                    rtn = "应用市场";
                    break;
                default:
                    break;
            }

            return rtn;
        }
    }
}