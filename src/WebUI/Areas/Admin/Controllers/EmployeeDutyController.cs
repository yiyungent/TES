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
using WebUI.Areas.Admin.Models.EmployeeDutyVM;
using WebUI.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    public class EmployeeDutyController : Controller
    {
        #region Ctor
        public EmployeeDutyController()
        {
            ViewBag.PageHeader = "员工职位";
            ViewBag.PageHeaderDescription = "员工职位";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
            };
        }
        #endregion

        #region 列表
        public ActionResult Index(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            Query(queryConditions);

            ListViewModel<EmployeeDuty> viewModel = new ListViewModel<EmployeeDuty>(queryConditions, pageIndex: pageIndex, pageSize: pageSize);
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
                    queryType.Text = "职位名称";
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
                default:
                    queryType.Text = "职位名称";
                    queryConditions.Add(Expression.Like("Name", query, MatchMode.Anywhere));
                    break;
            }
            ViewBag.Query = query;
            ViewBag.QueryType = queryType;
        }
        #endregion

        #region 删除
        public JsonResult Delete(int id)
        {
            try
            {
                Container.Instance.Resolve<EmployeeDutyService>().Delete(id);

                return Json(new { code = 1, message = "删除成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "删除失败" });
            }
        }
        #endregion

        #region 查看
        public ViewResult Detail(int id)
        {
            EmployeeDuty viewModel = Container.Instance.Resolve<EmployeeDutyService>().GetEntity(id);

            return View(viewModel);
        }
        #endregion

        #region 编辑
        [HttpGet]
        public ViewResult Edit(int id)
        {
            EmployeeDuty dbModel = Container.Instance.Resolve<EmployeeDutyService>().GetEntity(id);
            EmployeeDutyForEditViewModel viewModel = (EmployeeDutyForEditViewModel)dbModel;

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Edit(EmployeeDutyForEditViewModel inputModel)
        {
            try
            {
                // 数据格式效验
                if (ModelState.IsValid)
                {
                    #region 数据有效效验

                    #endregion

                    #region 输入模型 -> 数据库模型
                    EmployeeDuty dbModel = (EmployeeDuty)inputModel;
                    #endregion

                    Container.Instance.Resolve<EmployeeDutyService>().Edit(dbModel);

                    return Json(new { code = 1, message = "保存成功" });
                }
                else
                {
                    string errorMessage = ModelState.GetErrorMessage();
                    return Json(new { code = -1, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = -2, message = "保存失败" });
            }
        }
        #endregion

        #region 新增
        [HttpGet]
        public ViewResult Create()
        {
            EmployeeDutyForEditViewModel viewModel = new EmployeeDutyForEditViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Create(EmployeeDutyForEditViewModel inputModel)
        {
            try
            {
                // 数据格式效验
                if (ModelState.IsValid)
                {
                    #region 数据有效效验

                    #endregion

                    #region 输入模型 -> 数据库模型
                    EmployeeDuty dbModel = (EmployeeDuty)inputModel;
                    #endregion

                    Container.Instance.Resolve<EmployeeDutyService>().Create(dbModel);

                    return Json(new { code = 1, message = "添加成功" });
                }
                else
                {
                    string errorMessage = ModelState.GetErrorMessage();
                    return Json(new { code = -1, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = -2, message = "添加失败" });
            }
        }
        #endregion
    }
}