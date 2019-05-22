using Core;
using Domain;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Extensions
{
    public static class DomainExt
    {
        #region 用户，学生，员工之间的绑定
        public static StudentInfo GetBindStudent(this UserInfo userInfo)
        {
            StudentInfo rtn = null;
            rtn = Container.Instance.Resolve<StudentInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq("UID", userInfo.ID)
            }).FirstOrDefault();

            return rtn;
        }

        public static EmployeeInfo GetBindEmployee(this UserInfo userInfo)
        {
            EmployeeInfo rtn = null;
            rtn = Container.Instance.Resolve<EmployeeInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq("UID", userInfo.ID)
            }).FirstOrDefault();

            return rtn;
        }

        public static UserInfo GetBindUser(this StudentInfo studentInfo)
        {
            UserInfo rtn = null;
            rtn = Container.Instance.Resolve<UserInfoService>().GetEntity(studentInfo.UID ?? 0);

            return rtn;
        }

        public static UserInfo GetBindUser(this EmployeeInfo employeeInfo)
        {
            UserInfo rtn = null;
            rtn = Container.Instance.Resolve<UserInfoService>().GetEntity(employeeInfo.UID ?? 0);

            return rtn;
        }

        public static bool BindStudent(this UserInfo userInfo, string studentCode)
        {
            bool isSuccess = true;
            StudentInfo bind = Container.Instance.Resolve<StudentInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq("StudentCode", studentCode)
            }).FirstOrDefault();
            if (bind == null)
            {
                isSuccess = false;
            }
            else
            {
                bind.UID = userInfo.ID;
                Container.Instance.Resolve<StudentInfoService>().Edit(bind);
            }

            return isSuccess;
        }

        public static bool BindEmployee(this UserInfo userInfo, string employeeCode)
        {
            bool isSuccess = true;
            EmployeeInfo bind = Container.Instance.Resolve<EmployeeInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq("EmployeeCode", employeeCode)
            }).FirstOrDefault();
            if (bind == null)
            {
                isSuccess = false;
            }
            else
            {
                bind.UID = userInfo.ID;
                Container.Instance.Resolve<EmployeeInfoService>().Edit(bind);
            }

            return isSuccess;
        }

        public static void UnBindStudent(this UserInfo userInfo)
        {
            StudentInfo bind = Container.Instance.Resolve<StudentInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq("UID", userInfo.ID)
            }).FirstOrDefault();
            if (bind != null)
            {
                bind.UID = null;
                Container.Instance.Resolve<StudentInfoService>().Edit(bind);
            }
        }

        public static void UnBindEmployee(this UserInfo userInfo)
        {
            EmployeeInfo bind = Container.Instance.Resolve<EmployeeInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq("UID", userInfo.ID)
            }).FirstOrDefault();
            if (bind != null)
            {
                bind.UID = null;
                Container.Instance.Resolve<EmployeeInfoService>().Edit(bind);
            }
        }
        #endregion

        #region 此班级的学生
        public static IList<StudentInfo> StudentList(this ClazzInfo clazzInfo)
        {
            IList<StudentInfo> rtn = null;
            int clazzId = clazzInfo.ID;
            rtn = Container.Instance.Resolve<StudentInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq("ClazzInfo.ID", clazzId)
            });
            if (rtn == null)
            {
                rtn = new List<StudentInfo>();
            }

            return rtn;
        }
        #endregion

        #region 此班级学生的人数
        public static int StudentCount(this ClazzInfo clazzInfo)
        {
            int rtn = 0;
            int clazzId = clazzInfo.ID;
            rtn = Container.Instance.Resolve<StudentInfoService>().Count(Expression.Eq("ClazzInfo.ID", clazzId));

            return rtn;
        }
        #endregion
    }
}