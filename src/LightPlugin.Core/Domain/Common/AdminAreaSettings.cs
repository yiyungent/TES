﻿
using LightPlugin.Core.Configuration;

namespace LightPlugin.Core.Domain.Common
{
    public class AdminAreaSettings : ISettings
    {
        /// <summary>
        /// Default grid page size
        /// </summary>
        public int DefaultGridPageSize { get; set; }
        /// <summary>
        /// A comma-separated list of available grid page sizes
        /// </summary>
        public string GridPageSizes { get; set; }
        /// <summary>
        /// A value indicating whether to display product pictures in adin area
        /// </summary>
        public bool DisplayProductPictures { get; set; }
        /// <summary>
        /// Additional settings for rich editor
        /// </summary>
        public string RichEditorAdditionalSettings { get; set; }
        /// <summary>
        ///A value indicating whether to javascript is supported in rcih editor
        /// </summary>
        public bool RichEditorAllowJavaScript { get; set; }
    }
}