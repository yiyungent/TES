using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models.EvaConditionVM
{
    public class ClassEvaConViewModel
    {
        public IList<ClassEvaConItemViewModel> List { get; set; }
    }

    public class ClassEvaConItemViewModel
    {
        /// <summary>
        /// 评价人（学生）
        /// </summary>
        public StudentInfo Student { get; set; }

        /// <summary>
        /// 已评价的课程
        /// </summary>
        public IList<CourseInfo> EvaCourseList { get; set; }
    }
}