using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models.Clazz
{
    public class OptionItem
    {
        public int val { get; set; }

        public string selected { get; set; }

        public string text { get; set; }
    }

    public class CourseTableTr
    {
        public int courseTableId { get; set; }

        public IList<OptionItem> teacherSelect { get; set; }

        public IList<OptionItem> courseSelect { get; set; }
    }



}