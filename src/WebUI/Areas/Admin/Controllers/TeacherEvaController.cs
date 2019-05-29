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
        { }
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

        #region 该任务要评价的教师列表
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
                // 该员工 所在系的 所有员工
                // 只能评价 同行（同院同系）的教师
                viewModel = bindEmployee.Department.EmployeeInfoList();
            }
            // 查询这些 教师中哪些是 自己已经评价过的
            // 当前人当前评价任务 已经评价过的 教师ID 列表
            IList<int> isEvaedTeacherList = new List<int>();
            isEvaedTeacherList = Container.Instance.Resolve<EvaRecordService>().Query(new List<ICriterion>()
            {
                Expression.Eq("EvaluateTask.ID", id),
                Expression.Eq("Evaluator.ID", currentUser.ID)
            }).Select(m => m.ID).ToList();


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
            viewModel = GetNormTargetsByEvaorAndEvaedor(evaor, evaedEmployee, out normType);

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
                needAnswerNormTargetList = GetNormTargetsByEvaorAndEvaedor(evaor, evaedEmployee, out NormType normType);
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
                        NormType = new NormType { ID = 1 },
                        Options = new Options { ID = item.Value },
                        Evaluator = new UserInfo { ID = evaor.ID },
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

        #region Helpers

        #region 根据被评价人选择评价指标（需回答即有选项的指标）（答卷）
        /// <summary>
        /// 根据被评价人选择评价指标（答卷）
        /// </summary>
        /// <param name="evaedor">被评价员工（教师）</param>
        /// <param name="normType">他使用的评价类型</param>
        /// <returns>他使用的评价指标列表</returns>
        private IList<NormTarget> GetNormTargetsByEvaedor(EmployeeInfo evaedor, out NormType normType)
        {
            IList<NormTarget> rtn = null;
            normType = null;
            // 职位 区别
            switch (evaedor.Duty)
            {
                case 1:
                    // 普通教师
                    // 被评人 职位 为: "普通教师"， 使用  "系 （部） 方 面" 类型  的指标
                    normType = Container.Instance.Resolve<NormTypeService>().GetEntity(2);
                    rtn = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                        {
                            Expression.Eq("NormType.ID", normType.ID)
                        }).Where(m => m.OptionsList != null && m.OptionsList.Count >= 1).ToList();
                    break;
                case 2:
                    // 系主任
                    // 被评人 职位 为: "系主任"， 使用  "同行方面（领导）"  类型的指标
                    normType = Container.Instance.Resolve<NormTypeService>().GetEntity(4);
                    rtn = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                        {
                            Expression.Eq("NormType.ID", normType.ID)
                        }).Where(m => m.OptionsList != null && m.OptionsList.Count >= 1).ToList();
                    break;
                case 3:
                    // 

                    break;
            }

            return rtn;
        }
        #endregion

        #region 根据评价人和被评价人选择指标（需回答即有选项的末指标）（答卷）
        private IList<NormTarget> GetNormTargetsByEvaorAndEvaedor(UserInfo evaor, EmployeeInfo evaedor, out NormType normType)
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
                rtn = GetNormTargetsByEvaedor(evaedor, out normType);
            }

            return rtn;
        }
        #endregion

        #endregion
    }
}