using Core;
using Domain;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models
{
    public class CourseTableForEditViewModel
    {
        public int ID { get; set; }

        public List<OptionModel> ClazzOptions { get; set; }

        public List<OptionModel> CourseOptions { get; set; }

        public List<OptionModel> TeacherOptions { get; set; }

        public static explicit operator CourseTableForEditViewModel(CourseTable courseTable)
        {
            IList<ClazzInfo> allClazz = Container.Instance.Resolve<ClazzInfoService>().GetAll();
            List<OptionModel> clazzOptions = new List<OptionModel>();
            foreach (ClazzInfo item in allClazz)
            {
                clazzOptions.Add(new OptionModel
                {
                    ID = item.ID,
                    Text = item.ClazzCode,
                    IsSelected = courseTable.Clazz.ID == item.ID
                });
            }

            IList<CourseInfo> allCourse = Container.Instance.Resolve<CourseInfoService>().GetAll();
            List<OptionModel> courseOptions = new List<OptionModel>();
            foreach (CourseInfo item in allCourse)
            {
                courseOptions.Add(new OptionModel
                {
                    ID = item.ID,
                    Text = item.Name + "(" + item.CourseCode + ")",
                    IsSelected = courseTable.Course.ID == item.ID
                });
            }

            IList<EmployeeInfo> allTeacher = Container.Instance.Resolve<EmployeeInfoService>().GetAll();
            List<OptionModel> teacherOptions = new List<OptionModel>();
            foreach (EmployeeInfo item in allTeacher)
            {
                teacherOptions.Add(new OptionModel
                {
                    ID = item.ID,
                    Text = item.Name + "(" + item.EmployeeCode + ")",
                    IsSelected = courseTable.Teacher.ID == item.ID
                });
            }


            CourseTableForEditViewModel rtnModel = new CourseTableForEditViewModel
            {
                ID = courseTable.ID,
                ClazzOptions = clazzOptions,
                CourseOptions = courseOptions,
                TeacherOptions = teacherOptions
            };

            return rtnModel;
        }

        public static explicit operator CourseTable(CourseTableForEditViewModel model)
        {
            #region 班级
            int selectedClazzId = model.ClazzOptions[0].ID;
            ClazzInfo selectedClazz = Container.Instance.Resolve<ClazzInfoService>().Query(new List<ICriterion>
                    {
                        Expression.Eq("ID", selectedClazzId)
                    }).FirstOrDefault();
            #endregion

            #region 课程
            int selectedCourseId = model.CourseOptions[0].ID;
            CourseInfo selectedCourse = Container.Instance.Resolve<CourseInfoService>().Query(new List<ICriterion>
                    {
                        Expression.Eq("ID", selectedCourseId)
                    }).FirstOrDefault();
            #endregion

            #region 教师
            int selectedTeacherId = model.TeacherOptions[0].ID;
            EmployeeInfo selectedTeacher = Container.Instance.Resolve<EmployeeInfoService>().Query(new List<ICriterion>
                    {
                        Expression.Eq("ID", selectedTeacherId)
                    }).FirstOrDefault();
            #endregion

            CourseTable rtn = new CourseTable
            {
                ID = model.ID,
                Clazz = selectedClazz,
                Course = selectedCourse,
                Teacher = selectedTeacher
            };

            return rtn;
        }
    }

    #region 班级相等比较器
    public class ClazzInfoEqualityComparer : IEqualityComparer<ClazzInfo>
    {
        public bool Equals(ClazzInfo x, ClazzInfo y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            return x.ID == y.ID;
        }

        public int GetHashCode(ClazzInfo obj)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}