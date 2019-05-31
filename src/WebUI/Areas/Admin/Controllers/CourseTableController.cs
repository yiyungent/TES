using Core;
using Domain;
using Framework.HtmlHelpers;
using Framework.Models;
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
    public class CourseTableController : Controller
    {
        #region Ctor
        public CourseTableController()
        {
            ViewBag.PageHeader = "课程表管理";
            ViewBag.PageHeaderDescription = "课程表管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
                new BreadcrumbItem("课程表管理"),
            };
        }
        #endregion

        #region 列表
        public ActionResult Index(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            Query(queryConditions);

            ListViewModel<CourseTable> model = new ListViewModel<CourseTable>(queryConditions, pageIndex: pageIndex, pageSize: pageSize);
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(model);
        }

        private void Query(IList<ICriterion> queryConditions)
        {
            // 输入的查询关键词
            string query = Request["q"]?.Trim() ?? "";
            // 查询类型
            QueryType queryType = new QueryType();
            Dictionary<string, string> queryTypeValTextDic = new Dictionary<string, string>
            {
                { "clazzcode", "班号" },
                { "teachercode", "授课教师工号" },
                { "teachername", "授课教师姓名" },
                { "id", "ID" },
            };
            queryType.Val = Request["type"]?.Trim().ToLower() ?? "clazzcode";
            if (queryTypeValTextDic.ContainsKey(queryType.Val))
            {
                queryType.Text = queryTypeValTextDic[queryType.Val];
            }
            else
            {
                queryType.Text = "班号";
            }
            if (!string.IsNullOrEmpty(query))
            {
                switch (queryType.Val)
                {
                    case "clazzcode":
                        queryType.Text = "班号";
                        IList<ClazzInfo> clazzList = Container.Instance.Resolve<ClazzInfoService>().Query(new List<ICriterion>
                    {
                        Expression.Like("ClazzCode", query, MatchMode.Anywhere)
                    }).ToList();
                        queryConditions.Add(Expression.In("Clazz.ID", clazzList.Select(m => m.ID).ToArray()));
                        break;
                    case "teachercode":
                        queryType.Text = "授课教师工号";
                        IList<EmployeeInfo> teacherList = Container.Instance.Resolve<EmployeeInfoService>().Query(new List<ICriterion>
                    {
                        Expression.Like("EmployeeCode", query, MatchMode.Anywhere)
                    }).ToList();
                        queryConditions.Add(Expression.In("Teacher.ID", teacherList.Select(m => m.ID).ToArray()));
                        break;
                    case "teachername":
                        queryType.Text = "授课教师姓名";
                        teacherList = Container.Instance.Resolve<EmployeeInfoService>().Query(new List<ICriterion>
                    {
                        Expression.Like("Name", query, MatchMode.Anywhere)
                    }).ToList();
                        queryConditions.Add(Expression.In("Teacher.ID", teacherList.Select(m => m.ID).ToArray()));
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
                        queryType.Text = "班号";
                        clazzList = Container.Instance.Resolve<ClazzInfoService>().Query(new List<ICriterion>
                    {
                        Expression.Like("ClazzCode", query, MatchMode.Anywhere)
                    }).ToList();
                        queryConditions.Add(Expression.In("Clazz.ID", clazzList.Select(m => m.ID).ToArray()));
                        break;
                }
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
                Container.Instance.Resolve<CourseTableService>().Delete(id);

                return Json(new { code = 1, message = "删除成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 1, message = "删除失败" });
            }
        }
        #endregion

        #region 查看
        public ViewResult Detail(int id)
        {
            CourseTable model = Container.Instance.Resolve<CourseTableService>().GetEntity(id);

            return View(model);
        }
        #endregion

        #region 编辑
        [HttpGet]
        public ViewResult Edit(int id)
        {
            CourseTable courseTable = Container.Instance.Resolve<CourseTableService>().GetEntity(id);
            CourseTableForEditViewModel model = (CourseTableForEditViewModel)courseTable;

            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(CourseTableForEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CourseTable db = (CourseTable)model;

                    Container.Instance.Resolve<CourseTableService>().Edit(db);

                    return Json(new { code = 1, message = "保存成功" });
                }
                else
                {
                    return Json(new { code = -1, message = "不合理的输入" });
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
            CourseTable courseTable = new CourseTable
            {
                ID = 0,
                Clazz = new ClazzInfo(),
                Course = new CourseInfo(),
                Teacher = new EmployeeInfo()
            };
            CourseTableForEditViewModel model = (CourseTableForEditViewModel)courseTable;

            return View(model);
        }

        [HttpPost]
        public JsonResult Create(CourseTableForEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CourseTable dbModel = (CourseTable)model;

                    Container.Instance.Resolve<CourseTableService>().Create(dbModel);

                    return Json(new { code = 1, message = "添加成功" });
                }
                else
                {
                    string errorMessage = string.Empty;
                    foreach (ModelState item in ModelState.Values)
                    {
                        errorMessage += item.Errors.FirstOrDefault().ErrorMessage;
                    }
                    return Json(new { code = -1, message = "不合理的输入:" + errorMessage + ", " });
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