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
    }
}