using Domain;
using Framework.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models
{
    public class CourseInfoListViewModel
    {
        public IList<CourseInfo> CourseInfos { get; set; }

        public PageInfo PageInfo { get; set; }
    }
}