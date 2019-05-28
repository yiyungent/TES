using Core;
using Domain;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;
using WebUI.Areas.Admin.Models.Common;

namespace WebUI.Areas.Admin.Controllers
{
    public class EmployeeInfoController : Controller
    {
        #region Ctor
        public EmployeeInfoController()
        {
            ViewBag.PageHeader = "员工管理";
            ViewBag.PageHeaderDescription = "员工管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
                new BreadcrumbItem("员工管理"),
            };
        }
        #endregion

        #region 列表
        public ActionResult Index(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            Query(queryConditions);

            ListViewModel<EmployeeInfo> viewModel = new ListViewModel<EmployeeInfo>(queryConditions, pageIndex: pageIndex, pageSize: pageSize);
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
                    queryType.Text = "姓名";
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
                case "employeecode":
                    queryType.Text = "工号";
                    queryConditions.Add(Expression.Like("EmployeeCode", query, MatchMode.Anywhere));
                    break;
                case "departmentname":
                    queryType.Text = "部门名";
                    IList<Department> deptList = Container.Instance.Resolve<DepartmentService>().Query(new List<ICriterion>
                    {
                        Expression.Like("Name", query, MatchMode.Anywhere)
                    }).ToList();
                    queryConditions.Add(Expression.In("Department.ID", deptList.Select(m => m.ID).ToArray()));
                    break;
                default:
                    queryType.Text = "姓名";
                    queryConditions.Add(Expression.Like("Name", query, MatchMode.Anywhere));
                    break;
            }
            ViewBag.Query = query;
            ViewBag.QueryType = queryType;
        }
        #endregion
    }
}