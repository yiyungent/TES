using Common;
using Core;
using Domain;
using NHibernate.Criterion;
using PluginHub.Infrastructure;
using Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models.Common;
using WebUI.Areas.Admin.Models.EvaResultVM;
using WebUI.Infrastructure.CaculateScore;

namespace WebUI.Areas.Admin.Controllers
{
    public class EvaResultController : Controller
    {
        #region Fields

        ICaculateScore _caculateScore;

        #endregion

        #region Ctor
        public EvaResultController()
        {
            this._caculateScore = EngineContext.Current.Resolve<ICaculateScore>();
        }
        #endregion

        #region Methods

        #region 列表
        public ActionResult Index(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            Query(queryConditions);

            // 第一次搜索筛选行
            // 只显示 有评价记录（即可以计算分数）的选项列表
            IList<EvaRecord> allEvaRecord = Container.Instance.Resolve<EvaRecordService>().Query(queryConditions);

            #region 准备列表视图数据
            EvaResultListViewModel viewModel = new EvaResultListViewModel();
            IList<EvaResult> allEvaResult = Container.Instance.Resolve<EvaResultService>().GetAll();
            // 去重 + 只要有评价结果记录的

            IList<EvaTask> allRelativeEvaTask = allEvaResult.Select(m => m.EvaluateTask).Distinct().ToList();
            IList<EmployeeInfo> allEvaedEmployee = allEvaResult.Select(m => m.Teacher).Distinct().ToList();

            IList<EvaTask> allEvaTask = Container.Instance.Resolve<EvaTaskService>().GetAll();
            IList<EmployeeInfo> allEmployee = Container.Instance.Resolve<EmployeeInfoService>().GetAll();

            viewModel.List = new List<EvaResultVMItem>();
            foreach (var evaedEmployeeItem in allEmployee)
            {
                foreach (var evaTask in allEvaTask)
                {
                    EvaResultVMItem vmItem = new EvaResultVMItem();
                    vmItem.EvaedEmployee = evaedEmployeeItem;
                    vmItem.EvaTask = evaTask;
                    vmItem.ScoreDic = new Dictionary<NormType, decimal>();
                    // 只要 此评价任务，此被评员工 的评价记录
                    // 注意：表头的评价类型是按 排序码 排序，所以下方的明细分数单元格也许按排序码排序，以达明细分数一一对应
                    IList<EvaResult> relativeResultList = allEvaResult.Where(m => m.EvaluateTask.ID == vmItem.EvaTask.ID && m.Teacher.ID == vmItem.EvaedEmployee.ID).OrderBy(m => m.NormType.SortCode).ToList();
                    foreach (var resultItem in relativeResultList)
                    {
                        if (!vmItem.ScoreDic.ContainsKey(resultItem.NormType))
                        {
                            vmItem.ScoreDic.Add(resultItem.NormType, resultItem.Score);
                        }
                    }
                    if (!viewModel.List.Contains(vmItem, new EvaResultVMItemCompare()))
                    {
                        viewModel.Add(vmItem);
                    }
                }
            }
            #endregion

            #region 搜索二次筛选行--在视图模型准备完成后再次筛选

            SearchTwoFilter(viewModel);

            #endregion

            #region 页码

            viewModel.PageInfo = new Framework.HtmlHelpers.PageInfo
            {
                MaxLinkCount = 6,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRecordCount = viewModel.List.Count,
            };

            // 根据页码选择记录
            // 当前页号超过总页数，则显示最后一页
            int lastPageIndex = (int)Math.Ceiling((double)viewModel.List.Count / pageSize);
            pageIndex = pageIndex <= lastPageIndex ? pageIndex : lastPageIndex;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            // 使用 Skip 还顺便解决了 若 pageIndex <= 0 的错误情况
            var pageData = (from m in viewModel.List
                            orderby m.ID descending
                            select m).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            viewModel.List = pageData.ToList();

            #endregion

            #region 展示到视图

            // 表头：需要展示哪些 评价类型 的 明细分数
            IList<NormType> allNormType = Container.Instance.Resolve<NormTypeService>().GetAll().OrderBy(m => m.SortCode).ToList();
            ViewBag.AllNormType = allNormType;
            ViewBag.SelectListForEvaTask = InitSelectListForEvaTask(0, allEvaTask);
            TempData["RedirectUrl"] = Request.RawUrl;

            #endregion

            return View(viewModel);
        }

        #region 查询-搜索二次筛选
        private void SearchTwoFilter(EvaResultListViewModel viewModel)
        {
            // 输入的查询关键词
            string query = Request["q"]?.Trim() ?? "";
            // 查询类型
            QueryType queryType = new QueryType();
            Dictionary<string, string> queryTypeValTextDic = new Dictionary<string, string>
            {
                { "teachername", "教师名" },
                { "employeecode", "教师工号" },
                { "evataskname", "评价任务名" },
                { "scoresum", "总分" },
            };
            queryType.Val = Request["type"]?.Trim().ToLower() ?? "teachername";
            if (queryTypeValTextDic.ContainsKey(queryType.Val))
            {
                queryType.Text = queryTypeValTextDic[queryType.Val];
            }
            else
            {
                queryType.Text = "教师名";
            }
            if (!string.IsNullOrEmpty(query))
            {
                switch (queryType.Val)
                {
                    case "teachername":
                        viewModel.List = viewModel.List.Where(m => m.EvaedEmployee.Name.Contains(query)).ToList();
                        break;
                    case "employeecode":
                        viewModel.List = viewModel.List.Where(m => m.EvaedEmployee.EmployeeCode.Equals(query)).ToList();
                        break;
                    case "evataskname":
                        viewModel.List = viewModel.List.Where(m => m.EvaTask.Name.Contains(query)).ToList();
                        break;
                    case "scoresum":
                        if (decimal.TryParse(query, out decimal queryScoreSum))
                        {
                            viewModel.List = viewModel.List.Where(m => m.ScoreSum.ToString("0.##") == queryScoreSum.ToString("0.##")).ToList();
                        }
                        break;
                    default:
                        viewModel.List = viewModel.List.Where(m => m.EvaedEmployee.Name.Contains(query)).ToList();
                        break;
                }
            }
            ViewBag.Query = query;
            ViewBag.QueryType = queryType;
        }
        #endregion

        #region 查询
        private void Query(IList<ICriterion> queryConditions)
        {
            // 输入的查询关键词
            string query = Request["q"]?.Trim() ?? "";
            // 查询类型
            QueryType queryType = new QueryType();
            Dictionary<string, string> queryTypeValTextDic = new Dictionary<string, string>
            {
                { "teachername", "教师名" },
                { "evataskname", "评价任务名" },
            };
            queryType.Val = Request["type"]?.Trim().ToLower() ?? "teachername";
            if (queryTypeValTextDic.ContainsKey(queryType.Val))
            {
                queryType.Text = queryTypeValTextDic[queryType.Val];
            }
            else
            {
                queryType.Text = "教师名";
            }
            if (!string.IsNullOrEmpty(query))
            {
                switch (queryType.Val)
                {
                    case "teachername":
                        IList<EmployeeInfo> employeeList = Container.Instance.Resolve<EmployeeInfoService>().Query(new List<ICriterion>
                        {
                            Expression.Like("Name", query, MatchMode.Anywhere)
                        }).ToList();
                        queryConditions.Add(Expression.In("Teacher.ID", employeeList.Select(m => m.ID).ToArray()));
                        break;
                    case "evataskname":
                        IList<EvaTask> evaTaskList = Container.Instance.Resolve<EvaTaskService>().Query(new List<ICriterion>
                        {
                            Expression.Like("Name", query, MatchMode.Anywhere)
                        }).ToList();
                        queryConditions.Add(Expression.In("EvaluateTask.ID", evaTaskList.Select(m => m.ID).ToArray()));
                        break;
                    default:
                        employeeList = Container.Instance.Resolve<EmployeeInfoService>().Query(new List<ICriterion>
                        {
                            Expression.Like("Name", query, MatchMode.Anywhere)
                        }).ToList();
                        queryConditions.Add(Expression.In("Teacher.ID", employeeList.Select(m => m.ID).ToArray()));
                        break;
                }
            }
            ViewBag.Query = query;
            ViewBag.QueryType = queryType;
        }
        #endregion

        #endregion

        #region 计算分数
        [HttpPost]
        public JsonResult CaculateScore(int evaTaskId, int teacherId)
        {
            try
            {
                #region 有效性效验
                // 效验是否有 符合的 评价记录 以供 计算分数
                bool isExist = Container.Instance.Resolve<EvaRecordService>().Count(
                    Expression.Eq("EvaluateTask.ID", evaTaskId),
                    Expression.Eq("Teacher.ID", teacherId)
                 ) >= 1;
                if (!isExist)
                {
                    return Json(new { code = -1, message = "计算失败，没有符合的评价记录 以供计算" });
                }
                #endregion

                IList<EvaRecord> allRelativeRecord = Container.Instance.Resolve<EvaRecordService>().Query(new List<ICriterion>
                {
                    Expression.And(
                        Expression.Eq("EvaluateTask.ID", evaTaskId),
                        Expression.Eq("Teacher.ID", teacherId)
                    )
                });

                EvaTask evaTask = new EvaTask() { ID = evaTaskId };
                EmployeeInfo teacher = new EmployeeInfo() { ID = teacherId };
                IList<NormType> allNormType = allRelativeRecord.Select(m => m.NormType).Distinct().ToList();

                foreach (var normType in allNormType)
                {
                    _caculateScore.Caculate(evaTask, normType, teacher);
                }

                TempData["message"] = "计算成功";
                return Json(new { code = 1, message = "计算成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "计算失败" });
            }
        }
        #endregion

        #region 导出某评价任务的评价结果Excel
        public JsonResult ExportExcel(int evaTaskId)
        {
            try
            {
                EvaResultListViewModel viewModel = new EvaResultListViewModel();
                IList<EvaResult> allEvaResult = Container.Instance.Resolve<EvaResultService>().GetAll();
                // 去重 + 只要有评价结果记录的

                IList<EvaTask> allRelativeEvaTask = allEvaResult.Select(m => m.EvaluateTask).Distinct().ToList();
                IList<EmployeeInfo> allEvaedEmployee = allEvaResult.Select(m => m.Teacher).Distinct().ToList();

                IList<EvaTask> allEvaTask = Container.Instance.Resolve<EvaTaskService>().GetAll();
                IList<EmployeeInfo> allEmployee = Container.Instance.Resolve<EmployeeInfoService>().GetAll();

                IList<NormType> allNormType = Container.Instance.Resolve<NormTypeService>().GetAll();

                viewModel.List = new List<EvaResultVMItem>();
                foreach (var evaedEmployeeItem in allEmployee)
                {
                    foreach (var evaTask in allEvaTask)
                    {
                        EvaResultVMItem vmItem = new EvaResultVMItem();
                        vmItem.EvaedEmployee = evaedEmployeeItem;
                        vmItem.EvaTask = evaTask;
                        vmItem.ScoreDic = new Dictionary<NormType, decimal>();
                        // 只要 此评价任务，此被评员工 的评价记录
                        // 注意：表头的评价类型是按 排序码 排序，所以下方的明细分数单元格也许按排序码排序，以达明细分数一一对应
                        IList<EvaResult> relativeResultList = allEvaResult.Where(m => m.EvaluateTask.ID == vmItem.EvaTask.ID && m.Teacher.ID == vmItem.EvaedEmployee.ID).OrderBy(m => m.NormType.SortCode).ToList();
                        foreach (var normType in allNormType)
                        {
                            vmItem.ScoreDic.Add(normType, relativeResultList.Where(m => m.NormType.ID == normType.ID).Select(m => m.Score).FirstOrDefault());
                        }
                        if (!viewModel.List.Contains(vmItem, new EvaResultVMItemCompare()))
                        {
                            viewModel.Add(vmItem);
                        }
                    }
                }

                // 筛选-只要此评价任务
                viewModel.List = viewModel.List.Where(m => m.EvaTask.ID == evaTaskId).ToList();

                IList<EvaResultVMItem> excelDataList = viewModel.List;

                EvaTask currentEvaTask = allEvaTask.Where(m => m.ID == evaTaskId).FirstOrDefault();
                string saveFilePath = "/Resources/Excels/" + currentEvaTask.Name + "评价情况表.xls";
                if (!System.IO.File.Exists(Server.MapPath(saveFilePath)))
                {
                    System.IO.File.Create(Server.MapPath(saveFilePath)).Dispose();
                }

                // 将数据列表转换为 DataTable
                DataTable dataTable = EvaResultVMIListToDataTable(excelDataList);
                // 将 DataTable 保存到本地，返回文件路径
                NPOIHelper.DataTableToExcel(Server.MapPath(saveFilePath), dataTable, currentEvaTask.Name + "评价情况表", true);

                return Json(new { code = 1, message = "导出成功", fileUrl = saveFilePath }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "导出失败", fileUrl = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #endregion



        #region Helpers

        #region 初始化选项列表-评价任务
        /// <summary>
        /// 初始化选项列表-评价任务
        /// </summary>
        private static IList<SelectListItem> InitSelectListForEvaTask(int selectedValue, IList<EvaTask> allEvaTask)
        {
            IList<SelectListItem> ret = new List<SelectListItem>();
            ret.Add(new SelectListItem()
            {
                Text = "请选择",
                Value = "0",
                Selected = (selectedValue == 0)
            });
            foreach (var item in allEvaTask)
            {
                ret.Add(new SelectListItem()
                {
                    Text = $"{item.Name}（{item.EvaTaskCode}）",
                    Value = item.ID.ToString(),
                    Selected = (selectedValue == item.ID)
                });
            }

            return ret;
        }
        #endregion

        #region 评价结果视图项比较器
        sealed class EvaResultVMItemCompare : IEqualityComparer<EvaResultVMItem>
        {
            public bool Equals(EvaResultVMItem x, EvaResultVMItem y)
            {
                if (x == null || y == null)
                {
                    return false;
                }
                if (x.EvaTask.ID == y.EvaTask.ID && x.EvaedEmployee.ID == y.EvaedEmployee.ID)
                {
                    return true;
                }

                return false;
            }

            public int GetHashCode(EvaResultVMItem obj)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region 数据列表转换为 DataTable
        private DataTable EvaResultVMIListToDataTable(IList<EvaResultVMItem> excelDataList)
        {
            DataTable rtn = new DataTable();
            rtn.Columns.Add("评价任务");
            rtn.Columns.Add("被评价教师（员工）");
            IList<NormType> allNormType = Container.Instance.Resolve<NormTypeService>().GetAll().OrderBy(m => m.SortCode).ToList();
            foreach (var normType in allNormType)
            {
                rtn.Columns.Add(normType.Name, typeof(decimal));
            }
            rtn.Columns.Add("总分");

            foreach (var item in excelDataList)
            {
                IList<object> rowDatas = new List<object>();
                rowDatas.Add(item.EvaTask.Name);
                rowDatas.Add(item.EvaedEmployee.Name + "（" + item.EvaedEmployee.EmployeeCode + "）");
                // 循环添加 各评价类型分数，注意与表头的一一对应
                foreach (var normTypeAndScore in item.ScoreDic)
                {
                    rowDatas.Add(normTypeAndScore.Value);
                }
                rowDatas.Add(item.ScoreSum);

                rtn.Rows.Add(rowDatas.ToArray());
            }

            return rtn;
        }
        #endregion

        #endregion






    }
}