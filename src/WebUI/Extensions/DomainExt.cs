using Core;
using Domain;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

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

        #region 该部门的员工
        public static IList<EmployeeInfo> EmployeeInfoList(this Department department)
        {
            IList<EmployeeInfo> rtn = new List<EmployeeInfo>();
            rtn = Container.Instance.Resolve<EmployeeInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq("Department.ID", department.ID)
            }).ToList();

            return rtn;
        }
        #endregion

        #region int实际意义 与 枚举转换
        public static string ToEmployeeDuty(this int value)
        {
            string meanStr = string.Empty;
            switch (value)
            {
                case 1:
                    meanStr = "教师";
                    break;
                case 2:
                    meanStr = "系主任";
                    break;
                default:
                    meanStr = "未知";
                    break;
            }

            return meanStr;
        }

        public static string ToEvaTaskStatus(this int value)
        {
            string meanStr = string.Empty;
            switch (value)
            {
                case 1:
                    meanStr = "待开启";
                    break;
                case 2:
                    meanStr = "正在评价";
                    break;
                case 3:
                    meanStr = "评价结束";
                    break;
                default:
                    meanStr = "未知";
                    break;
            }

            return meanStr;
        }
        #endregion

        #region 通过路由获取系统菜单
        public static Sys_Menu GetSysMenuByRoute(string areaName, string controllerName, string actionName)
        {
            Sys_Menu rtn = null;
            rtn = Container.Instance.Resolve<Sys_MenuService>().Query(new List<ICriterion>
            {
                Expression.Like("AreaName", areaName, MatchMode.Anywhere),
                Expression.Like("ControllerName", controllerName, MatchMode.Anywhere),
                Expression.Like("ActionName", actionName, MatchMode.Anywhere)
            }).FirstOrDefault();
            if (rtn == null)
            {
                // 如果没有 此 ActionName 对应的系统菜单，则忽视 ActionName 重查
                rtn = Container.Instance.Resolve<Sys_MenuService>().Query(new List<ICriterion>
                {
                    Expression.Like("AreaName", areaName, MatchMode.Anywhere),
                    Expression.Like("ControllerName", controllerName, MatchMode.Anywhere),
                }).FirstOrDefault();
            }

            return rtn;
        }

        public static Sys_Menu GetSysMenuByRoute(this RouteData routeData)
        {
            Sys_Menu rtn = null;
            string areaName = routeData.DataTokens["area"]?.ToString() ?? "";
            string controllerName = routeData.Values["controller"]?.ToString() ?? "";
            string actionName = routeData.Values["action"]?.ToString() ?? "";
            rtn = GetSysMenuByRoute(areaName, controllerName, actionName);

            return rtn;
        }
        #endregion
    }

    public enum EmployeeDuty
    {
        教师 = 1,
        系主任 = 2
    }
}