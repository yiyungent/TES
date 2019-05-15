using Core;
using Domain;
using Framework.HtmlHelpers;
using Framework.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;
using WebUI.Areas.Admin.Models.Clazz;
using WebUI.Areas.Admin.Models.Common;

namespace WebUI.Areas.Admin.Controllers
{
    public class ClazzInfoController : Controller
    {
        #region Ctor
        public ClazzInfoController()
        {
            ViewBag.PageHeader = "班级管理";
            ViewBag.PageHeaderDescription = "班级管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
            };
        }
        #endregion

        #region 列表
        public ActionResult Index(CurrentAccountModel currentAccount, int pageIndex = 1, int pageSize = 6)
        {
            ListViewModel<ClazzInfo> model = new ListViewModel<ClazzInfo>(pageIndex: pageIndex, pageSize: pageSize);
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(model);
        }
        #endregion


        #region 调课
        /// <summary>
        /// 调课
        /// </summary>
        /// <param name="id">班级ID</param>
        /// <returns></returns>
        public ViewResult AssignCourse(int id)
        {
            #region 暂时放弃
            //ClazzInfo clazzInfo = Container.Instance.Resolve<ClazzInfoService>().GetEntity(id);

            //ClazzForAssignCourseViewModel model = new ClazzForAssignCourseViewModel();
            //model.ClazzID = clazzInfo.ID;
            //model.AssignCourseOptionList = new List<AssignCourseItem>();

            //for (int i = 0; i < clazzInfo.CourseTableList.Count; i++)
            //{
            //    CourseTable courseTable = clazzInfo.CourseTableList[i];
            //    AssignCourseItem assignCourseItem = (AssignCourseItem)courseTable;
            //    model.AssignCourseOptionList.Add(assignCourseItem);
            //} 
            #endregion
            ClazzInfo clazzInfo = Container.Instance.Resolve<ClazzInfoService>().GetEntity(id);
            ViewBag.CurrentClazz = clazzInfo;

            TempData["RedirectUrl"] = Request.RawUrl;

            return View();
        }
        #endregion

        #region 获取指定班级的课程表
        /// <summary>
        /// 获取指定班级的课程表
        /// </summary>
        /// <param name="id">班级ID</param>
        public JsonResult GetCourseTableList(int id)
        {
            IList<CourseTableTr> rtnJson = new List<CourseTableTr>();

            // 筛选出当前班级的 所有课程表
            IList<CourseTable> clazzCourseTableList = Container.Instance.Resolve<CourseTableService>().GetAll().Where(m => m.Clazz.ID == id).ToList();
            IList<CourseInfo> allCourseList = Container.Instance.Resolve<CourseInfoService>().GetAll();
            IList<EmployeeInfo> allEmployeeList = Container.Instance.Resolve<EmployeeInfoService>().GetAll();

            foreach (CourseTable item in clazzCourseTableList)
            {
                CourseTableTr tr = new CourseTableTr();
                tr.courseSelect = new List<OptionItem>();
                foreach (CourseInfo cItem in allCourseList)
                {
                    OptionItem option = new OptionItem();
                    if (cItem.ID == item.Course.ID)
                    {
                        option.selected = "selected";
                    }
                    else
                    {
                        option.selected = "";
                    }
                    option.text = $"{cItem.Name} ({cItem.CourseCode})";
                    option.val = cItem.ID;
                    tr.courseSelect.Add(option);
                }

                tr.teacherSelect = new List<OptionItem>();
                foreach (EmployeeInfo eItem in allEmployeeList)
                {
                    OptionItem option = new OptionItem();
                    if (eItem.ID == item.Teacher.ID)
                    {
                        option.selected = "selected";
                    }
                    else
                    {
                        option.selected = "";
                    }
                    option.text = $"{eItem.Name} ({eItem.EmployeeCode})";
                    option.val = eItem.ID;
                    tr.teacherSelect.Add(option);
                }

                rtnJson.Add(tr);
            }

            return Json(rtnJson, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}