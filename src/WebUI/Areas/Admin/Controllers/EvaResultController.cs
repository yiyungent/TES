using Core;
using Domain;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models.Common;
using WebUI.Areas.Admin.Models.EvaResultVM;

namespace WebUI.Areas.Admin.Controllers
{
    public class EvaResultController : Controller
    {
        #region 列表
        public ActionResult Index(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            Query(queryConditions);


            // 只显示 有评价记录（即可以计算分数）的选项列表
            IList<EvaRecord> allEvaRecord = Container.Instance.Resolve<EvaRecordService>().GetAll();

            #region 准备列表视图数据
            EvaResultListViewModel viewModel = new EvaResultListViewModel();
            viewModel.List = new List<EvaResultVMItem>();
            IList<EvaResult> allEvaResult = Container.Instance.Resolve<EvaResultService>().GetAll();
            // 去重 + 只要有评价结果记录的
            IList<EvaTask> allRelativeEvaTask = allEvaResult.Select(m => m.EvaluateTask).Distinct().ToList();
            IList<EmployeeInfo> allEvaedEmployee = allEvaResult.Select(m => m.Teacher).Distinct().ToList();
            foreach (var evaedEmployeeItem in allEvaedEmployee)
            {
                EvaResultVMItem vmItem = new EvaResultVMItem();
                vmItem.EvaTask = allEvaResult.Where(m => m.Teacher.ID == evaedEmployeeItem.ID).Select(m => m.EvaluateTask).FirstOrDefault();
                vmItem.EvaedEmployee = evaedEmployeeItem;
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

                viewModel.List.Add(vmItem);
            }
            #endregion


            #region 展示到视图

            #region 计算分数的选项列表
            IList<EvaTask> allEvaTask = Container.Instance.Resolve<EvaTaskService>().GetAll();
            ViewBag.SelectListForEvaTask = InitSelectListForEvaTask(0, allEvaTask);
            #endregion

            // 表头：需要展示哪些 评价类型 的 明细分数
            IList<NormType> allNormType = Container.Instance.Resolve<NormTypeService>().GetAll().OrderBy(m => m.SortCode).ToList();
            ViewBag.AllNormType = allNormType;

            TempData["RedirectUrl"] = Request.RawUrl;
            #endregion

            return View(viewModel);
        }

        private void Query(IList<ICriterion> queryConditions)
        {
            // 输入的查询关键词
            string query = Request["q"]?.Trim() ?? "";
            // 查询类型
            QueryType queryType = new QueryType();
            Dictionary<string, string> queryTypeValTextDic = new Dictionary<string, string>
            {
                { "teachername", "教师名" },
                { "evatypename", "评价类型名" },
                { "evataskname", "评价任务名" },
                { "score", "分数" },
                { "id", "ID" },
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
                    case "evatypename":
                        IList<NormType> normTypeList = Container.Instance.Resolve<NormTypeService>().Query(new List<ICriterion>
                        {
                            Expression.Like("Name", query, MatchMode.Anywhere)
                        }).ToList();
                        queryConditions.Add(Expression.In("NormType.ID", normTypeList.Select(m => m.ID).ToArray()));
                        break;
                    case "evataskname":
                        IList<EvaTask> evaTaskList = Container.Instance.Resolve<EvaTaskService>().Query(new List<ICriterion>
                        {
                            Expression.Like("Name", query, MatchMode.Anywhere)
                        }).ToList();
                        queryConditions.Add(Expression.In("EvaTask.ID", evaTaskList.Select(m => m.ID).ToArray()));
                        break;
                    case "score":
                        if (decimal.TryParse(query, out decimal queryScore))
                        {
                            queryConditions.Add(Expression.Eq("Score", queryScore));
                        }
                        break;
                    case "id":
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
                        IList<EmployeeInfo> employeeList_2 = Container.Instance.Resolve<EmployeeInfoService>().Query(new List<ICriterion>
                        {
                            Expression.Like("Name", query, MatchMode.Anywhere)
                        }).ToList();
                        queryConditions.Add(Expression.In("Teacher.ID", employeeList_2.Select(m => m.ID).ToArray()));
                        break;
                }
            }
            ViewBag.Query = query;
            ViewBag.QueryType = queryType;
        }
        #endregion

        #region 计算分数
        /// <summary>
        /// 计算此任务的 所有评价类型，所有教师
        /// </summary>
        /// <param name="evaTaskId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CaculateScore(int evaTaskId)
        {
            try
            {
                #region 有效性效验
                // 效验是否有 符合的 评价记录 以供 计算分数
                bool isExist = Container.Instance.Resolve<EvaRecordService>().Count(
                    Expression.Eq("EvaluateTask.ID", evaTaskId)
                 ) >= 1;
                if (!isExist)
                {
                    return Json(new { code = -1, message = "计算失败，没有符合的评价记录 以供计算" });
                }
                #endregion

                IList<EvaRecord> allRelativeEvaRecord = Container.Instance.Resolve<EvaRecordService>().Query(new List<ICriterion>
                {
                    Expression.Eq("EvaluateTask.ID", evaTaskId)
                });

                //IList<EmployeeInfo> allTeacher = Container.Instance.Resolve<EmployeeInfoService>().GetAll();
                IList<EmployeeInfo> allTeacher = allRelativeEvaRecord.Select(m => m.Teacher).ToList();
                //IList<NormType> allNormType = Container.Instance.Resolve<NormTypeService>().GetAll();
                IList<NormType> allNormType = allRelativeEvaRecord.Select(m => m.NormType).ToList();
                //EvaTask EvaTask = Container.Instance.Resolve<EvaTaskService>().GetEntity(evaTaskId);
                EvaTask evaTask = new EvaTask { ID = evaTaskId };
                foreach (var teacher in allTeacher)
                {
                    foreach (var normType in allNormType)
                    {
                        Caculate(evaTask, normType, teacher);
                    }
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



        #region Helpers

        #region 计算分数并保存评价结果
        private void Caculate(EvaTask evaTask, NormType normType, EmployeeInfo employeeInfo)
        {
            IList<NormTarget> allTarget = Container.Instance.Resolve<NormTargetService>().GetAll();
            IList<EvaRecord> allRecord = Container.Instance.Resolve<EvaRecordService>().Query(new List<ICriterion>
            {
                Expression.Eq("EvaluateTask.ID", evaTask.ID),
                Expression.Eq("NormType.ID", normType.ID),
                Expression.Eq("Teacher.ID", employeeInfo.ID)
            });

            // 按评价人排序
            //allRecord = allRecord.OrderBy(m => m.SysUser.ID).ToList();

            // 计算分数和+参评人员
            int personCount = 0; // 参评人数
            int userId = 0; // 评价人ID，用于计算参评人数
            decimal sumScore = 0;
            decimal avgScore = 0;
            foreach (var item in allRecord)
            {
                if (item.Evaluator.ID != userId)
                {
                    personCount = personCount + 1;

                    userId = item.Evaluator.ID;
                }
                // 递归确定权重（含归一化处理）
                decimal weight = GetWeight(item.NormTarget, allTarget);

                sumScore = sumScore + item.Options.Score * weight;
            }
            // 求平均数
            if (personCount != 0)
            {
                avgScore = sumScore / personCount;
            }
            // 判断是否已经存在 此评价结果-创建 / 更新（重新计算）
            EvaResult existEvaResult = Container.Instance.Resolve<EvaResultService>().Query(
                new List<ICriterion> {
                    Expression.Eq("Teacher.ID", employeeInfo.ID),
                    Expression.Eq("NormType.ID", normType.ID),
                    Expression.Eq("EvaluateTask.ID", evaTask.ID)
            }
            ).FirstOrDefault();
            if (existEvaResult != null)
            {
                // 更新 计算结果EvaResult
                existEvaResult.CaculateTime = DateTime.Now;
                existEvaResult.Score = avgScore;
                Container.Instance.Resolve<EvaResultService>().Edit(existEvaResult);
            }
            else
            {
                // 创建 计算结果EvaResult
                Container.Instance.Resolve<EvaResultService>().Create(new EvaResult
                {
                    Teacher = employeeInfo,
                    NormType = normType,
                    CaculateTime = DateTime.Now, // 计算分数时间
                    EvaluateTask = evaTask,
                    Score = avgScore
                });
            }
        }
        #endregion

        #region 递归获取权重（含归一化处理）
        private decimal GetWeight(NormTarget self, IList<NormTarget> all)
        {
            if (self.ParentTarget == null)
            {
                var findBrother = from m in all
                                  where m.ParentTarget == null
                                  && m.NormType != null
                                  && self.NormType != null
                                  && m.NormType.ID == self.NormType.ID
                                  select m;

                decimal sumWeight = 0;
                foreach (var item in findBrother)
                {
                    sumWeight = sumWeight + item.Weight;
                }

                if (sumWeight == 0) sumWeight = 1; // 避免0做除数

                return self.Weight / sumWeight;
            }
            else
            {
                var findBrother = from m in all
                                  where m.ParentTarget != null
                                  && m.ParentTarget != null
                                  && self.ParentTarget != null
                                  && m.ParentTarget.ID == self.ParentTarget.ID
                                  && m.NormType != null
                                  && self.NormType != null
                                  && m.NormType.ID == self.NormType.ID
                                  select m;
                decimal sumWeight = 0;
                foreach (var item in findBrother)
                {
                    sumWeight = sumWeight + item.Weight;
                }
                if (sumWeight == 0) sumWeight = 1;

                // 需要递归
                return self.Weight / sumWeight * GetWeight(self.ParentTarget, all);
            }
        }
        #endregion

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

        #endregion





    }
}