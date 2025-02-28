﻿using Core;
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
using WebUI.Areas.Admin.Models.Common;
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

        #region 评价任务列表
        public ViewResult Index(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            Query(queryConditions);

            ListViewModel<EvaTask> viewModel = new ListViewModel<EvaTask>(queryConditions, pageIndex: pageIndex, pageSize: pageSize);
            // 只显示 "正在评价"，"评价结束" 状态的 评价任务
            viewModel.List = viewModel.List.Where(m => m.Status == 2 || m.Status == 3).ToList();
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(viewModel);
        }

        private void Query(IList<ICriterion> queryConditions)
        {
            // 输入的查询关键词
            string query = Request["q"]?.Trim() ?? "";
            // 查询类型
            QueryType queryType = new QueryType();
            queryType.Val = Request["type"]?.Trim() ?? "name";
            switch (queryType.Val.ToLower())
            {
                case "name":
                    queryType.Text = "任务名称";
                    queryConditions.Add(Expression.Like("Name", query, MatchMode.Anywhere));
                    break;
                case "id":
                    queryType.Text = "ID";
                    if (!string.IsNullOrEmpty(query))
                    {
                        if (int.TryParse(query, out int id))
                        {
                            queryConditions.Add(Expression.Eq("ID", id));
                        }
                        else
                        {
                            queryConditions.Add(Expression.Eq("ID", 0));
                        }
                    }
                    break;
                case "evataskcode":
                    queryType.Text = "任务号";
                    queryConditions.Add(Expression.Like("EvaTaskCode", query, MatchMode.Anywhere));
                    break;
                default:
                    queryType.Text = "任务名称";
                    queryConditions.Add(Expression.Like("Name", query, MatchMode.Anywhere));
                    break;
            }
            ViewBag.Query = query;
            ViewBag.QueryType = queryType;
        }
        #endregion

        #region 学生要评价的 课程和教师列表
        /// <summary>
        /// 需评价列表
        /// </summary>
        /// <param name="id">关联的 评价任务ID</param>
        /// <returns></returns>
        public ViewResult EvaList(int id)
        {
            UserInfo currentUser = AccountManager.GetCurrentUserInfo();
            StudentInfo bindStudent = currentUser.GetBindStudent();
            IList<CourseTable> viewModel = null;
            if (bindStudent != null)
            {
                // 该学生 所在班级的 课表
                viewModel = bindStudent.ClazzInfo.CourseTableList;
            }
            // 查询这些 教师中哪些是 自己已经评价过的
            // 当前人当前评价任务 已经评价过的 教师ID 列表
            IList<int> isEvaedTeacherList = new List<int>();
            isEvaedTeacherList = Container.Instance.Resolve<EvaRecordService>().Query(new List<ICriterion>()
            {
                Expression.Eq("EvaluateTask.ID", id),
                Expression.Eq("Evaluator.ID", currentUser.ID)
            }).Select(m => m.Teacher.ID).ToList();

            #region 展示到视图
            ViewBag.CurrentStudent = bindStudent;
            ViewBag.CurrentClazz = bindStudent?.ClazzInfo;
            ViewBag.EvaTaskId = id;
            EvaTask evaTask = Container.Instance.Resolve<EvaTaskService>().GetEntity(id);
            ViewBag.EvaTask = evaTask;
            ViewBag.IsEvaedTeacherList = isEvaedTeacherList;
            TempData["RedirectUrl"] = Request.RawUrl;
            #endregion

            return View(viewModel);
        }
        #endregion

        #region 评价
        /// <summary>
        /// 评价
        /// </summary>
        /// <param name="id">课表ID</param>
        /// <param name="evaTaskId">评价任务ID</param>
        /// <returns></returns>
        [HttpGet]
        public ViewResult Eva(int courseTableId, int evaTaskId)
        {
            // 学生评价教师 使用 "学生评价" 类型的指标
            IList<NormTarget> viewModel = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
            {
                Expression.Eq("NormType.ID", 1)
            });
            CourseTable courseTable = Container.Instance.Resolve<CourseTableService>().GetEntity(courseTableId);
            ViewBag.CourseTable = courseTable;
            ViewBag.EvaTaskId = evaTaskId;

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Eva(int courseId, int teacherId, int evaTaskId, bool falg = false)
        {
            try
            {
                #region 安全起见
                // 只允许评价 "正在评价" 状态 的评价任务
                EvaTask evaTask = Container.Instance.Resolve<EvaTaskService>().GetEntity(evaTaskId);
                if (evaTask == null || evaTask.Status == 1 || evaTask.Status == 3)
                {
                    return Json(new { code = -1, message = "评价失败，当前评价任务不存在，或未开启，或已结束" });
                }
                #endregion

                // 评价人（当前登录人）
                UserInfo evaor = AccountManager.GetCurrentUserInfo();

                // "学生方面" 指标
                IList<NormTarget> needAnswerNormTargetList = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("NormType.ID", 1)
                }).Where(m => m.OptionsList != null && m.OptionsList.Count >= 1).ToList();
                Dictionary<int, int> normTargetIdAndSelectedOptionDic = new Dictionary<int, int>();
                // 提交正确的指标选中项 计数
                int count = 0;
                foreach (var item in needAnswerNormTargetList)
                {
                    if (int.TryParse(Request["normTarget_" + item.ID], out int selectedOptionId))
                    {
                        normTargetIdAndSelectedOptionDic.Add(item.ID, selectedOptionId);
                        count++;
                    }
                }
                if (count < needAnswerNormTargetList.Count)
                {
                    return Json(new { code = -2, message = "请做完评价" });
                }

                foreach (var item in normTargetIdAndSelectedOptionDic)
                {
                    Container.Instance.Resolve<EvaRecordService>().Create(new EvaRecord
                    {
                        EvaDate = DateTime.Now,
                        NormTarget = new NormTarget { ID = item.Key },
                        NormType = new NormType { ID = 1 },
                        Options = new Options { ID = item.Value },
                        Evaluator = new UserInfo { ID = evaor.ID },
                        Teacher = new EmployeeInfo { ID = teacherId },
                        EvaluateTask = new EvaTask { ID = evaTaskId },
                        ClazzInfo = evaor.GetBindStudent().ClazzInfo,
                        CourseInfo = new CourseInfo { ID = courseId }
                    });
                }

                return Json(new { code = 1, message = "提交评价成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "提交评价失败" });
            }
        }
        #endregion

        #region 评价成功
        public ViewResult EvaSuccess()
        {
            return View();
        }
        #endregion


    }
}