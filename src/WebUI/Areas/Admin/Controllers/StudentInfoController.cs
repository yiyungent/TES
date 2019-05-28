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
using WebUI.Areas.Admin.Models.StudentInfoVM;
using WebUI.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    public class StudentInfoController : Controller
    {
        #region Ctor
        public StudentInfoController()
        {
            ViewBag.PageHeader = "学生管理";
            ViewBag.PageHeaderDescription = "学生管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
                new BreadcrumbItem("学生管理"),
            };
        }
        #endregion

        #region 列表
        public ActionResult Index(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            Query(queryConditions);

            ListViewModel<StudentInfo> viewModel = new ListViewModel<StudentInfo>(queryConditions, pageIndex: pageIndex, pageSize: pageSize);
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
                case "studentcode":
                    queryType.Text = "学号";
                    queryConditions.Add(Expression.Like("StudentCode", query, MatchMode.Anywhere));
                    break;
                case "clazzcode":
                    queryType.Text = "班号";
                    queryConditions.Add(Expression.Like("ClazzInfo.ClazzCode", query, MatchMode.Anywhere));
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
                Container.Instance.Resolve<StudentInfoService>().Delete(id);

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
            StudentInfo viewModel = Container.Instance.Resolve<StudentInfoService>().GetEntity(id);

            return View(viewModel);
        }
        #endregion

        #region 编辑
        [HttpGet]
        public ViewResult Edit(int id)
        {
            StudentInfo dbModel = Container.Instance.Resolve<StudentInfoService>().GetEntity(id);
            StudentInfoForEditViewModel viewModel = (StudentInfoForEditViewModel)dbModel;

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Edit(StudentInfoForEditViewModel inputModel)
        {
            try
            {
                // 数据格式效验
                if (ModelState.IsValid)
                {
                    #region 数据有效效验
                    if (Container.Instance.Resolve<StudentInfoService>().Exist(inputModel.InputStudentCode.Trim(), inputModel.ID))
                    {
                        return Json(new { code = -1, message = "此学号已被其它学生使用，请更换" });
                    }
                    #endregion

                    #region 输入模型 -> 数据库模型
                    StudentInfo dbModel = (StudentInfo)inputModel;
                    #endregion

                    Container.Instance.Resolve<StudentInfoService>().Edit(dbModel);

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
            StudentInfoForEditViewModel viewModel = new StudentInfoForEditViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Create(StudentInfoForEditViewModel inputModel)
        {
            try
            {
                // 数据格式效验
                if (ModelState.IsValid)
                {
                    #region 数据有效效验
                    // 查找 已经有此代码的
                    if (Container.Instance.Resolve<StudentInfoService>().Exist(inputModel.InputStudentCode))
                    {
                        return Json(new { code = -3, message = "学号已经存在, 请更换" });
                    }
                    #endregion

                    #region 输入模型 -> 数据库模型
                    StudentInfo dbModel = (StudentInfo)inputModel;
                    #endregion

                    Container.Instance.Resolve<StudentInfoService>().Create(dbModel);

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