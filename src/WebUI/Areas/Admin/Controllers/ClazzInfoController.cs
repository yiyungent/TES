using Core;
using Domain;
using Framework.HtmlHelpers;
using Framework.Models;
using Newtonsoft.Json;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        [HttpGet]
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

            return View();
        }

        [HttpPost]
        public JsonResult SaveCourseTableList(int clazzId, IList<ClazzAssignCourseItemJson> listJson)
        {
            try
            {
                IList<ClazzAssignCourseItemJson> items = listJson;
                //IList<ClazzAssignCourseItemJson> items = JsonConvert.DeserializeObject<IList<ClazzAssignCourseItemJson>>(listJson);
                ClazzInfo clazzInfo = Container.Instance.Resolve<ClazzInfoService>().GetEntity(clazzId);
                IList<CourseInfo> allCourse = Container.Instance.Resolve<CourseInfoService>().GetAll();
                IList<EmployeeInfo> allEmployee = Container.Instance.Resolve<EmployeeInfoService>().GetAll();

                IList<CourseTable> courseTableList = new List<CourseTable>();
                IList<CourseTable> addCourseTableList = new List<CourseTable>();
                foreach (ClazzAssignCourseItemJson item in items)
                {
                    if (item.courseTableId != 0)
                    {
                        // 非新增-更新
                        courseTableList.Add(new CourseTable
                        {
                            Clazz = clazzInfo,
                            Course = allCourse.Where(m => m.ID == item.courseId).FirstOrDefault(),
                            Teacher = allEmployee.Where(m => m.ID == item.teacherId).FirstOrDefault(),
                            ID = item.courseTableId
                        });
                    }
                    else
                    {
                        // 新增行
                        addCourseTableList.Add(new CourseTable
                        {
                            Clazz = clazzInfo,
                            Course = allCourse.Where(m => m.ID == item.courseId).FirstOrDefault(),
                            Teacher = allEmployee.Where(m => m.ID == item.teacherId).FirstOrDefault(),
                            ID = item.courseTableId
                        });
                    }
                }

                clazzInfo.CourseTableList = courseTableList;
                // 更新原有
                Container.Instance.Resolve<ClazzInfoService>().Edit(clazzInfo);
                // 新增行课表
                foreach (CourseTable item in addCourseTableList)
                {
                    Container.Instance.Resolve<CourseTableService>().Create(item);
                }
                // BUG: 当删除某行 CourseTable 时，导致 此 CourseTable 的 clazzId 为 空，出现脏数据
                #region 临时删除脏数据
                // 临时解决
                // 查询出所有 clazzId 为 null 的 CourseTable
                IList<CourseTable> allDeletedCourseTableList = Container.Instance.Resolve<CourseTableService>().Query(new List<ICriterion>
                    {
                        Expression.IsNull("Clazz.ID")
                    });
                for (int i = 0; i < allDeletedCourseTableList.Count; i++)
                {
                    Container.Instance.Resolve<CourseTableService>().Delete(allDeletedCourseTableList[i]);
                }
                #region MyRegion
                //Thread th = new Thread(() =>
                //{

                //})
                //{ IsBackground = true };
                //th.Start(); 
                #endregion
                #endregion

                return Json(new { code = 1, message = "保存成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "保存失败" });
            }
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
            try
            {
                // 筛选出当前班级的 所有课程表
                IList<CourseTable> clazzCourseTableList = Container.Instance.Resolve<CourseTableService>().GetAll().Where(m => m.Clazz.ID == id).ToList();
                IList<CourseInfo> allCourseList = Container.Instance.Resolve<CourseInfoService>().GetAll();
                IList<EmployeeInfo> allEmployeeList = Container.Instance.Resolve<EmployeeInfoService>().GetAll();

                foreach (CourseTable item in clazzCourseTableList)
                {
                    CourseTableTr tr = new CourseTableTr();
                    tr.courseTableId = item.ID;

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

            }
            catch (Exception ex)
            {

            }
            return Json(rtnJson, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取所有课程json
        public JsonResult GetAllCourseJson()
        {
            IList<CourseInfo> allCourse = Container.Instance.Resolve<CourseInfoService>().GetAll();
            var allCourseJson = from m in allCourse
                                select new { text = $"{m.Name} ({m.CourseCode})", val = m.ID };

            return Json(allCourseJson);
        }
        #endregion

        #region 获取所有员工(教师)json
        public JsonResult GetAllTeacherJson()
        {
            IList<EmployeeInfo> allEmployee = Container.Instance.Resolve<EmployeeInfoService>().GetAll();
            var allTeacherJson = from m in allEmployee
                                 select new { text = $"{m.Name} ({m.EmployeeCode})", val = m.ID };

            return Json(allTeacherJson);
        }
        #endregion

    }
}