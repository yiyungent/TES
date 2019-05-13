using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Domain;

namespace WebUI.Areas.Admin.Models
{
    public class CourseInfoForEditViewModel
    {
        [Required]
        public int ID { get; set; }

        [Display(Name = "课程名")]
        [Required(ErrorMessage = "课程名不能为空")]
        public string InputName { get; set; }

        [Display(Name = "课程代号")]
        [Required(ErrorMessage = "课程代号不能为空")]
        public string InputCourseCode { get; set; }

        #region Helper
        public static explicit operator CourseInfoForEditViewModel(CourseInfo courseInfo)
        {
            CourseInfoForEditViewModel rtnModel = new CourseInfoForEditViewModel
            {
                ID = courseInfo.ID,
                InputName = courseInfo.Name,
                InputCourseCode = courseInfo.CourseCode
            };

            return rtnModel;
        }

        public static explicit operator CourseInfo(CourseInfoForEditViewModel model)
        {
            CourseInfo rtnModel = new CourseInfo
            {
                ID = model.ID,
                Name = model.InputName,
                CourseCode = model.InputCourseCode
            };

            return rtnModel;
        }
        #endregion
    }
}