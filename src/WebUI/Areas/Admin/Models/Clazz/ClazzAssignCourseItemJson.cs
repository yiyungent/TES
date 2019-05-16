using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models.Clazz
{
    public class ClazzAssignCourseItemJson
    {
        public int courseTableId { get; set; }
        public int courseId { get; set; }
        public int teacherId { get; set; }
    }
}