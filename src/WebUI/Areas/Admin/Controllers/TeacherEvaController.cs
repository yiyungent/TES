using Core;
using Domain;
using Framework.Infrastructure.Concrete;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;
using WebUI.Areas.Admin.Models.Common;
using WebUI.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    public class TeacherEvaController : Controller
    {
        #region Ctor
        public TeacherEvaController()
        {
            ViewBag.PageHeader = "教师评价";
            ViewBag.PageHeaderDescription = "教师评价";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("评价管理"),
                new BreadcrumbItem("教师评价"),
            };
        }
        #endregion

        #region 评价任务列表
        public ViewResult Index(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            Query(queryConditions);

            ListViewModel<EvaTask> viewModel = new ListViewModel<EvaTask>(queryConditions, pageIndex: pageIndex, pageSize: pageSize);
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

        #region 该任务要评价的列表
        /// <summary>
        /// 需评价列表
        /// </summary>
        /// <param name="id">关联的 评价任务ID</param>
        /// <returns></returns>
        public ActionResult EvaList(int id)
        {
            UserInfo currentUser = AccountManager.GetCurrentUserInfo();
            EmployeeInfo bindEmployee = currentUser.GetBindEmployee();
            IList<EmployeeInfo> viewModel = null;
            if (bindEmployee != null)
            {
                switch (bindEmployee.Duty)
                {
                    case 1:
                        // 教师

                        break;
                    case 2:
                        // 系主任

                        break;
                }
                // 该员工 所在部门的 所有员工
                viewModel = bindEmployee.Department.EmployeeInfoList();
            }
            ViewBag.CurrentEmployee = bindEmployee;
            ViewBag.CurrentDept = bindEmployee?.Department;
            ViewBag.EvaTaskId = id;
            EvaTask evaTask = Container.Instance.Resolve<EvaTaskService>().GetEntity(id);
            ViewBag.EvaTask = evaTask;
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(viewModel);
        }
        #endregion

        #region 评价
        /// <summary>
        /// 评价
        /// </summary>
        /// <param name="id">被评价 教师ID</param>
        /// <param name="evaTaskId">对应的 评价任务ID</param>
        /// <returns></returns>
        [HttpGet]
        public ViewResult Eva(int teacherId, int evaTaskId)
        {
            // 教师评价 同部门教师 使用 "同行方面" 类型的指标
            IList<NormTarget> viewModel = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
            {
                Expression.Eq("NormType.ID", 4)
            });
            EmployeeInfo employee = Container.Instance.Resolve<EmployeeInfoService>().GetEntity(teacherId);
            ViewBag.Employee = employee;
            ViewBag.EvaTaskId = evaTaskId;

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Eva(int teacherId, int evaTaskId, bool falg = false)
        {
            try
            {
                // "同行方面" 类型指标
                IList<NormTarget> needAnswerNormTargetList = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("NormType.ID", 4)
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
                        Teacher = new EmployeeInfo { ID = teacherId },
                        EvaluateTask = new EvaTask { ID = evaTaskId }
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