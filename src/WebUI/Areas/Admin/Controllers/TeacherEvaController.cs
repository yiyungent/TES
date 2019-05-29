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
        public ActionResult EvaList(int id)
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
            ViewBag.CurrentEmployee = bindEmployee;
            ViewBag.CurrentDept = bindEmployee?.Department;
            ViewBag.EvaTaskId = id;
            EvaTask evaTask = Container.Instance.Resolve<EvaTaskService>().GetEntity(id);
            ViewBag.EvaTask = evaTask;
            TempData["RedirectUrl"] = Request.RawUrl;

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
            EmployeeInfo employee = AccountManager.GetCurrentUserInfo().GetBindEmployee();
            // 被评人
            EmployeeInfo evaedEmployee = Container.Instance.Resolve<EmployeeInfoService>().GetEntity(teacherId);

            IList<NormTarget> viewModel = null;

            // 使用评价类型
            NormType normType = null;

            if (evaedEmployee.ID == employee?.ID)
            {
                // 被评人为: 当前登录人(自己)， 使用 "教师个人方面" 类型的指标
                normType = Container.Instance.Resolve<NormTypeService>().GetEntity(5);
                viewModel = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("NormType.ID", normType.ID)
                });
            }
            else
            {
                // 被评人 非自己
                // 职位 区别
                switch (evaedEmployee.Duty)
                {
                    case 1:
                        // 普通教师
                        // 被评人 职位 为: "普通教师"， 使用  "系 （部） 方 面" 类型  的指标
                        normType = Container.Instance.Resolve<NormTypeService>().GetEntity(2);
                        viewModel = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                        {
                            Expression.Eq("NormType.ID", normType.ID)
                        });
                        break;
                    case 2:
                        // 系主任
                        // 被评人 职位 为: "系主任"， 使用  "同行方面（领导）"  类型的指标
                        normType = Container.Instance.Resolve<NormTypeService>().GetEntity(4);
                        viewModel = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                        {
                            Expression.Eq("NormType.ID", normType.ID)
                        });
                        break;
                    case 3:
                        // 

                        break;
                }
            }

            // 展示 被评信息 到页面
            ViewBag.EvaedEmployee = evaedEmployee;
            ViewBag.NormType = normType;
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