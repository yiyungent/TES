using Core;
using Domain;
using Framework.Infrastructure.Concrete;
using Framework.Models;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Account.Models;
using WebUI.Areas.Admin.Models;
using WebUI.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    public class StudentEvaController : Controller
    {
        #region Ctor
        public StudentEvaController()
        {
            ViewBag.PageHeader = "学生评价";
            ViewBag.PageHeaderDescription = "学生评价";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("评价管理"),
                new BreadcrumbItem("学生评价"),
            };
        }
        #endregion

        #region 学生要评价的 课程和教师列表
        public ActionResult Index()
        {
            UserInfo currentUser = AccountManager.GetCurrentUserInfo();
            StudentInfo bindStudent = currentUser.GetBindStudent();
            IList<CourseTable> viewModel = null;
            if (bindStudent != null)
            {
                // 该学生 所在班级的 课表
                viewModel = bindStudent.ClazzInfo.CourseTableList;
            }
            ViewBag.CurrentStudent = bindStudent;
            ViewBag.CurrentClazz = bindStudent.ClazzInfo;
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(viewModel);
        }
        #endregion

        #region 评价
        /// <summary>
        /// 评价
        /// </summary>
        /// <param name="id">课表ID</param>
        /// <returns></returns>
        [HttpGet]
        public ViewResult Eva(int id)
        {
            // 学生评价教师 使用 "学生评价" 类型的指标
            IList<NormTarget> viewModel = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
            {
                Expression.Eq("NormType.ID", 1)
            });
            CourseTable courseTable = Container.Instance.Resolve<CourseTableService>().GetEntity(id);
            ViewBag.CourseTable = courseTable;

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Eva(int teacherId, bool flag = false)
        {
            try
            {
                // "学生方面" 指标
                IList<NormTarget> normTargetList = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("NormType.ID", 1)
                });
                foreach (var item in normTargetList)
                {
                    if (int.TryParse(Request["normTarget_" + item.ID], out int selectedOptionId))
                    {
                        Container.Instance.Resolve<EvaRecordService>().Create(new EvaRecord
                        {
                            EvaDate = DateTime.Now,
                            NormTarget = item,
                            NormType = new NormType { ID = 1 },
                            Options = new Options { ID = selectedOptionId },
                            Teacher = new EmployeeInfo { ID = teacherId },
                            EvaluateTask = null
                        });
                    }
                }

                return Json(new { code = 1, message = "提交评价成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "提交评价失败" });
            }
        }
        #endregion


    }
}