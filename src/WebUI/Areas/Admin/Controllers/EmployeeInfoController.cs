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
using WebUI.Areas.Admin.Models.EmployeeInfoVM;
using WebUI.Extensions;

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

        #region 删除
        public JsonResult Delete(int id)
        {
            try
            {
                Container.Instance.Resolve<EmployeeInfoService>().Delete(id);

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
            EmployeeInfo viewModel = Container.Instance.Resolve<EmployeeInfoService>().GetEntity(id);

            return View(viewModel);
        }
        #endregion

        #region 编辑
        [HttpGet]
        public ViewResult Edit(int id)
        {
            EmployeeInfo dbModel = Container.Instance.Resolve<EmployeeInfoService>().GetEntity(id);
            EmployeeInfoForEditViewModel viewModel = (EmployeeInfoForEditViewModel)dbModel;

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Edit(EmployeeInfoForEditViewModel inputModel)
        {
            try
            {
                // 数据格式效验
                if (ModelState.IsValid)
                {
                    #region 数据有效效验
                    if (Container.Instance.Resolve<EmployeeInfoService>().Exist(inputModel.InputEmployeeCode.Trim(), inputModel.ID))
                    {
                        return Json(new { code = -1, message = "此工号已被其它员工使用，请更换" });
                    }
                    #endregion

                    #region 输入模型 -> 数据库模型
                    EmployeeInfo dbModel = (EmployeeInfo)inputModel;
                    #endregion

                    Container.Instance.Resolve<EmployeeInfoService>().Edit(dbModel);

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
            EmployeeInfoForEditViewModel viewModel = new EmployeeInfoForEditViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Create(EmployeeInfoForEditViewModel inputModel)
        {
            try
            {
                // 数据格式效验
                if (ModelState.IsValid)
                {
                    #region 数据有效效验
                    // 查找 已经有此代码的
                    if (Container.Instance.Resolve<EmployeeInfoService>().Exist(inputModel.InputEmployeeCode))
                    {
                        return Json(new { code = -3, message = "工号已经存在, 请更换" });
                    }
                    #endregion

                    #region 输入模型 -> 数据库模型
                    EmployeeInfo dbModel = (EmployeeInfo)inputModel;
                    #endregion

                    Container.Instance.Resolve<EmployeeInfoService>().Create(dbModel);

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

        #region 评价分析
        /// <summary>
        /// 评价分析
        /// </summary>
        /// <param name="id">被评价人（员工）ID</param>
        [HttpGet]
        public ViewResult EvaAnalyze(int id)
        {
            EmployeeInfo viewModel = Container.Instance.Resolve<EmployeeInfoService>().GetEntity(id);

            return View(viewModel);
        }

        /// <summary>
        /// 评价分析
        /// </summary>
        /// <param name="id">被评价人（员工）ID</param>
        [HttpPost]
        public JsonResult EvaAnalyze(int id, string chartType)
        {

            IList<EvaTask> allEvaTask = Container.Instance.Resolve<EvaTaskService>().GetAll();
            IList<NormType> allNormType = Container.Instance.Resolve<NormTypeService>().GetAll().OrderBy(m => m.SortCode).ToList();
            EmployeeInfo evaedEmployee = Container.Instance.Resolve<EmployeeInfoService>().GetEntity(id);
            IList<EvaResult> allRelativeEvaResult = Container.Instance.Resolve<EvaResultService>().Query(new List<ICriterion>
            {
                Expression.Eq("Teacher.ID", id)
            });

            EvaAnalyzeChart chartOption = new EvaAnalyzeChart();
            #region 图标选项-基础设置
            chartOption.title = new Title() { text = "评价 - 堆叠区域图" };
            chartOption.tooltip = new Tooltip()
            {
                trigger = "axis",
                axisPointer = new Axispointer()
                {
                    type = "cross",
                    label = new Label()
                    {
                        backgroundColor = "#6a7985"
                    }
                }
            };
            chartOption.legend = new Legend()
            {
                data = allNormType.Select(m => m.Name).ToArray()
            };
            chartOption.toolbox = new Toolbox()
            {
                feature = new Feature()
                {
                    saveAsImage = new Saveasimage()
                }
            };
            chartOption.grid = new Grid()
            {
                left = "3%",
                right = "4%",
                bottom = "3%",
                containLabel = true
            };
            chartOption.xAxis = new Xaxi[]
            {
                new Xaxi()
                {
                    type = "category",
                    boundaryGap = false,
                    data = allEvaTask.Select(m=>m.Name).ToArray()
                }
            };
            chartOption.yAxis = new Yaxi[]
            {
                new Yaxi()
                {
                    type = "value"
                }
            };
            #endregion
            #region 图标选项-数据
            chartOption.series = new Series[allNormType.Count];
            for (int i = 0; i < allNormType.Count; i++)
            {
                chartOption.series[i] = new Series()
                {
                    name = allNormType[i].Name,
                    type = "line",
                    stack = "总量",
                    areaStyle = new Areastyle(),
                    // 筛选出 当前评价类型， 只要此对应的分数 属性数组
                    data = allRelativeEvaResult.Where(m => m.NormType.ID == allNormType[i].ID).OrderBy(m => m.NormType.SortCode).Select(m => m.Score).ToArray()
                };
            }
            #endregion


            return Json(chartOption);
        }
        #endregion
    }
}