﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaiduTJ.Models
{
    public class ConfigurationViewModel
    {
        [AllowHtml]
        public string TJCode { get; set; }
    }
}