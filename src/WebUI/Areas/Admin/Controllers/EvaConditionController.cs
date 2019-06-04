using Core;
using Domain;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models.Common;
using WebUI.Areas.Admin.Models.EvaConditionVM;
using WebUI.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    public class EvaConditionController : Controller
    {
        #region 评价任务列表
        public ActionResult Index(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            EvaTaskQuery(queryConditions);

            ListViewModel<EvaTask> viewModel = new ListViewModel<EvaTask>(queryConditions, pageIndex: pageIndex, pageSize: pageSize);
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(viewModel);
        }

        private void EvaTaskQuery(IList<ICriterion> queryConditions)
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

        #region 班级列表
        public ViewResult ClassList(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            Query(queryConditions);

            ListViewModel<ClazzInfo> model = new ListViewModel<ClazzInfo>(queryConditions, pageIndex: pageIndex, pageSize: pageSize);
            TempData["RedirectUrl"] = Request.RawUrl;
            int evaTaskId = int.Parse(Request["evaTaskId"]);
            ViewBag.EvaTask = Container.Instance.Resolve<EvaTaskService>().GetEntity(evaTaskId);

            return View(model);
        }

        #region 查询
        private void Query(IList<ICriterion> queryConditions)
        {
            // 输入的查询关键词
            string query = Request["q"]?.Trim() ?? "";
            // 查询类型
            string queryType = Request["type"]?.Trim() ?? "";
            switch (queryType.ToLower())
            {
                case "clazzcode":
                    queryConditions.Add(Expression.Like("ClazzCode", query, MatchMode.Anywhere));
                    break;
                case "id":
                    queryConditions.Add(Expression.Eq("ID", int.Parse(query)));
                    break;
                default:
                    queryConditions.Add(Expression.Like("ClazzCode", query, MatchMode.Anywhere));
                    break;
            }
            ViewBag.Query = query;
        }
        #endregion

        #endregion

        #region 此班级 此评价任务 的评价情况
        public ViewResult ClassEvaCon(int clazzId, int evaTaskId)
        {



            // 此任务，此班级的 评价记录
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            IList<EvaRecord> evaRecords = Container.Instance.Resolve<EvaRecordService>().Query(new List<ICriterion>
            {
                Expression.And(
                    Expression.Eq("EvaluateTask.ID", evaTaskId),
                    Expression.Eq("ClazzInfo.ID", clazzId)
                )
            });
            stopwatch.Stop();
            long totalTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();
            stopwatch.Start();
            // 此班级的所有学生
            IList<StudentInfo> studentInfoList = evaRecords.FirstOrDefault().ClazzInfo.StudentList();
            stopwatch.Stop();
            totalTime = stopwatch.ElapsedMilliseconds;

            ClazzInfo clazzInfo = studentInfoList.FirstOrDefault().ClazzInfo;

            // 有评价记录的用户
            IList<UserInfo> evaUserInfoList = evaRecords.Select(m => m.Evaluator).ToList();

            ClassEvaConViewModel viewModel = new ClassEvaConViewModel();
            viewModel.List = new List<ClassEvaConItemViewModel>();
            foreach (var student in studentInfoList)
            {
                viewModel.List.Add(new ClassEvaConItemViewModel
                {
                    Student = student,
                    // 该学生 评价的课程
                    EvaCourseList = evaRecords.Where(m => m.Evaluator.ID == student.GetBindUser().ID).Select(m => m.CourseInfo).Distinct().ToList()
                });
            }


            EvaTask evaTask = evaRecords.FirstOrDefault().EvaluateTask;

            ViewBag.EvaTask = evaTask;
            ViewBag.ClazzInfo = clazzInfo;

            return View(viewModel);
        }
        #endregion


    }
}