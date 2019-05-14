using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models.Template
{
    public class ThemeTemplateViewModel
    {
        public Source Source { get; set; }

        /// <summary>
        /// 服务器相对位置
        /// <para>eg. ~/Upload/Templates/Red.zip</para>
        /// </summary>
        public string ServerPath { get; set; }

        public string TemplateName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IList<string> Authors { get; set; }

        public string Version { get; set; }

        public string Url { get; set; }

        public bool IsDefault { get; set; }

        /// <summary>
        /// 状态
        /// <para>1: 启用</para>
        /// <para>0: 禁用</para>
        /// </summary>
        public int Status { get; set; }

        public string PreviewImageUrl
        {
            get
            {
                string url = "/Templates/" + TemplateName + "/preview.png";
                return url;
            }
        }
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