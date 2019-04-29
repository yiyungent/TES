using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class AdminWebBaseInfoViewModel
    {
        public string Logo_mini { get; set; }
        public string Logo_lg { get; set; }
        public string Copyright { get; set; }

        public AdminWebBaseInfoViewModel()
        {
            this.Logo_mini = "<b>S</b>tore";
            this.Logo_lg = "<b>DotKit</b>Store";
            this.Copyright = "<strong>Copyright &copy; " + DateTime.Now.Year + " <a href=\"#\">Company</a>.</strong> All rights reserved.";
        }
    }
}