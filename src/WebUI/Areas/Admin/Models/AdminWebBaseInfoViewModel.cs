using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models
{
    public class AdminWebBaseInfoViewModel
    {
        public string Logo_mini { get; set; }
        public string Logo_lg { get; set; }
        public string Copyright { get; set; }

        public AdminWebBaseInfoViewModel()
        {
            this.Logo_mini = "<b>T</b>ES";
            this.Logo_lg = "<b>教学</b>评价系统";
            this.Copyright = "<strong>Copyright &copy; " + DateTime.Now.Year + " <a href=\"#\">Company</a>.</strong> All rights reserved.";
        }
    }
}