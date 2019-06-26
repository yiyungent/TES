using Core;
using Domain;
using Framework.Infrastructure.Concrete;
using NHibernate.Criterion;
using PluginHub.Infrastructure;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;
using WebUI.Areas.Admin.Models.Common;
using WebUI.Areas.Admin.Models.EvaResultVM;
using WebUI.Extensions;
using WebUI.Infrastructure.CaculateScore;

namespace WebUI.Areas.Admin.Controllers
{
    public class TeacherEvaController : Controller
    {
        #region Fields

        private ICaculateScore _caculateScore;

        #endregion

        #region Ctor
        public TeacherEvaController()
        {
            this._caculateScore = EngineContext.Current.Resolve<ICaculateScore>();
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

        #region 该任务要评价的员工列表
        /// <summary>
        /// 需评价列表
        /// </summary>
        /// <param name="id">关联的 评价任务ID</param>
        /// <returns></returns>
        public ViewResult EvaList(int id)
        {
            UserInfo currentUser = AccountManager.GetCurrentUserInfo();
            EmployeeInfo bindEmployee = currentUser.GetBindEmployee();
            IList<EmployeeInfo> viewModel = null;
            if (bindEmployee != null)
            {
                #region 根据部门筛选
                // 只能评价 相同部门下(递归:包括后代所有部门) 的员工
                viewModel = GetAllEmployeeInSelfAndLaterDept(bindEmployee.Department);
                #endregion

                #region 根据职位筛选
                if (bindEmployee.EmployeeDuty != null)
                {
                    int currentEmployeeDutyId = bindEmployee.EmployeeDuty.ID;
                    IList<EmployeeDuty> evaDutyList = Container.Instance.Resolve<EmployeeDutyService>().GetEntity(currentEmployeeDutyId).EvaDutyList;
                    if (evaDutyList != null && evaDutyList.Count >= 1)
                    {
                        // 当前登录员工可以评价这些职位ID  的员工
                        IList<int> evaDutyIdList = evaDutyList.Select(m => m.ID).ToList();
                        // 筛选员工-只能评价这些职位的员工
                        viewModel = viewModel.Where(m => evaDutyIdList.Contains(m.EmployeeDuty?.ID ?? 0)).ToList();
                    }
                    else
                    {
                        // 职位约束：不能评任何人
                        viewModel = new List<EmployeeInfo>();
                    }
                }
                else
                {
                    // 没职位的员工 不能评任何人
                    viewModel = new List<EmployeeInfo>();
                }
                #endregion
            }



            #region 标记已评
            // 查询这些 教师中哪些是 自己已经评价过的
            // 当前人当前评价任务 已经评价过的 教师ID 列表
            IList<int> isEvaedTeacherList = new List<int>();
            isEvaedTeacherList = Container.Instance.Resolve<EvaRecordService>().Query(new List<ICriterion>()
            {
                Expression.Eq("EvaluateTask.ID", id),
                Expression.Eq("Evaluator.ID", currentUser.ID)
            }).Select(m => m.Teacher.ID).ToList();
            #endregion

            #region 展示到视图
            ViewBag.CurrentEmployee = bindEmployee;
            ViewBag.CurrentDept = bindEmployee?.Department;
            ViewBag.EvaTaskId = id;
            EvaTask evaTask = Container.Instance.Resolve<EvaTaskService>().GetEntity(id);
            ViewBag.EvaTask = evaTask;
            ViewBag.IsEvaedTeacherList = isEvaedTeacherList;
            TempData["RedirectUrl"] = Request.RawUrl;
            #endregion

            return View(viewModel);
        }
        #endregion

        #region 评价答卷
        /// <summary>
        /// 评价
        /// </summary>
        /// <param name="id">被评价 教师ID</param>
        /// <param name="evaTaskId">对应的 评价任务ID</param>
        /// <returns></returns>
        [HttpGet]
        public ViewResult Eva(int teacherId, int evaTaskId)
        {
            // 评价人（当前登录人）
            UserInfo evaor = AccountManager.GetCurrentUserInfo();
            // 被评人
            EmployeeInfo evaedEmployee = Container.Instance.Resolve<EmployeeInfoService>().GetEntity(teacherId);

            IList<NormTarget> viewModel = null;
            // 使用的评价类型
            NormType normType = null;
            // 根据 评价人，被评价人 选择 指标
            viewModel = GetNormTargetsByEvaorAndEvaedor(evaor.GetBindEmployee(), evaedEmployee, out normType);

            #region 展示 被评信息 到页面
            ViewBag.EvaedEmployee = evaedEmployee;
            ViewBag.NormType = normType;
            ViewBag.EvaTaskId = evaTaskId;
            #endregion

            return View(viewModel);
        }

        /// <summary>
        /// 评价答卷提交
        /// </summary>
        /// <param name="teacherId">被评 教师（员工）ID</param>
        /// <param name="evaTaskId">评价任务ID</param>
        /// <param name="falg">仅用作区别同名方法</param>
        [HttpPost]
        public JsonResult Eva(int teacherId, int evaTaskId, bool falg = false)
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
                // 被评人
                EmployeeInfo evaedEmployee = Container.Instance.Resolve<EmployeeInfoService>().GetEntity(teacherId);

                IList<NormTarget> needAnswerNormTargetList = null;
                needAnswerNormTargetList = GetNormTargetsByEvaorAndEvaedor(evaor.GetBindEmployee(), evaedEmployee, out NormType normType);
                if (needAnswerNormTargetList == null || needAnswerNormTargetList.Count <= 0)
                {
                    return Json(new { code = -1, message = "提交评价失败，没有需要回答的指标" });
                }

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
                        NormType = new NormType { ID = normType.ID },
                        Options = new Options { ID = item.Value },
                        Evaluator = new UserInfo { ID = evaor.ID },
                        Teacher = new EmployeeInfo { ID = teacherId },
                        EvaluateTask = new EvaTask { ID = evaTaskId }
                    });
                }

                //#region 计算分数
                //Task.Run(() =>
                //{
                //    Thread.Sleep(1000);
                //    _caculateScore.Caculate(evaTaskId, normType.ID, teacherId);
                //});
                //#endregion

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

        #region 查看评价
        public ViewResult EvaResult(int evaTaskId, int employeeId)
        {
            EmployeeInfo employee = Container.Instance.Resolve<EmployeeInfoService>().GetEntity(employeeId);
            EvaTask evaTask = Container.Instance.Resolve<EvaTaskService>().GetEntity(evaTaskId);
            IList<EvaResult> evaResults = Container.Instance.Resolve<EvaResultService>().Query(new List<ICriterion>
            {
                Expression.And(
                    Expression.Eq("EvaluateTask.ID", evaTaskId),
                    Expression.Eq("Teacher.ID", employeeId)
                )
            }).ToList();

            EvaResultViewModel viewModel = new EvaResultViewModel()
            {
                EmployeeInfo = employee,
                EvaTask = evaTask,
                EvaResultList = evaResults
            };

            return View(viewModel);
        }
        #endregion

        #region Helpers

        #region 根据评价人选择评价指标（需回答即有选项的指标）（答卷）
        /// <summary>
        /// 根据评价人选择评价指标（答卷）
        /// </summary>
        /// <param name="evaor">评价员工（教师）</param>
        /// <param name="normType">使用的评价类型</param>
        /// <returns>使用的评价指标列表</returns>
        private IList<NormTarget> GetNormTargetsByEvaor(EmployeeInfo evaor, out NormType normType)
        {
            IList<NormTarget> rtn = null;
            if (evaor.EmployeeDuty != null)
            {
                normType = evaor.EmployeeDuty.NormType;
                rtn = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("NormType.ID", normType.ID)
                }).Where(m => m.OptionsList != null && m.OptionsList.Count >= 1).ToList();
            }
            else
            {
                // 评价人无职位--->无评价类型-->无指标
                normType = null;
                rtn = new List<NormTarget>();
            }

            return rtn;
        }
        #endregion

        #region 根据评价人和被评价人选择指标（需回答即有选项的末指标）（答卷）
        /// <summary>
        /// 根据评价人和被评价人选择指标（需回答即有选项的末指标）（答卷）
        /// </summary>
        /// <param name="evaor">评价人</param>
        /// <param name="evaedor">被评价人</param>
        /// <param name="normType">带出使用的评价类型</param>
        /// <returns>使用的评价指标列表</returns>
        private IList<NormTarget> GetNormTargetsByEvaorAndEvaedor(EmployeeInfo evaor, EmployeeInfo evaedor, out NormType normType)
        {
            IList<NormTarget> rtn = null;
            normType = null;
            if (evaedor.ID == evaor?.ID)
            {
                // 被评人为: 当前登录人(自己)， 使用 "教师个人方面" 类型的指标
                normType = Container.Instance.Resolve<NormTypeService>().GetEntity(5);
                rtn = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("NormType.ID", normType.ID)
                }).Where(m => m.OptionsList != null && m.OptionsList.Count >= 1).ToList();
            }
            else
            {
                // 被评人 非自己
                // 职位 区别
                rtn = GetNormTargetsByEvaor(evaor, out normType);
            }

            return rtn;
        }
        #endregion

        #region 获取当前部门的所有后代部门（返回包括自身(当前)）
        private void GetAllLaterAndSelfDept(Department currentDept, IList<Department> resultDeptList)
        {
            resultDeptList.Add(currentDept);

            if (currentDept.Children != null && currentDept.Children.Count >= 1)
            {
                foreach (var item in currentDept.Children)
                {
                    GetAllLaterAndSelfDept(item, resultDeptList);
                }
            }
        }
        #endregion

        #region 获取这些部门的员工
        private IList<EmployeeInfo> GetAllEmployeeInDeptList(IList<Department> deptList)
        {
            IList<EmployeeInfo> rtn = new List<EmployeeInfo>();
            foreach (var dept in deptList)
            {
                foreach (var employee in dept.EmployeeInfoList())
                {
                    rtn.Add(employee);
                }
            }

            return rtn;
        }
        #endregion

        #region 获取当前部门的所有后代部门（包括自身）内的所有员工
        private IList<EmployeeInfo> GetAllEmployeeInSelfAndLaterDept(Department dept)
        {
            IList<Department> deptList = new List<Department>();
            GetAllLaterAndSelfDept(dept, deptList);

            return GetAllEmployeeInDeptList(deptList);
        }
        #endregion

        #endregion
    }
}