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
    public class CourseInfoController : Controller
    {
        #region Ctor
        public CourseInfoController()
        {
            ViewBag.PageHeader = "课程管理";
            ViewBag.PageHeaderDescription = "课程管理";
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

            ListViewModel<CourseInfo> model = new ListViewModel<CourseInfo>(queryConditions, pageIndex: pageIndex, pageSize: pageSize);
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(model);
        }
        #endregion

        #region 删除
        public JsonResult Delete(int id)
        {
            try
            {
                Container.Instance.Resolve<CourseInfoService>().Delete(id);

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
            CourseInfo model = Container.Instance.Resolve<CourseInfoService>().GetEntity(id);

            return View(model);
        }
        #endregion

        #region 编辑
        [HttpGet]
        public ViewResult Edit(int id)
        {
            CourseInfo courseInfo = Container.Instance.Resolve<CourseInfoService>().GetEntity(id);
            CourseInfoForEditViewModel model = (CourseInfoForEditViewModel)courseInfo;

            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(CourseInfoForEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CourseInfo dbEntry = Container.Instance.Resolve<CourseInfoService>().GetEntity(model.ID);
                    dbEntry.Name = model.InputName?.Trim();

                    // 查找 已经具有此课程名的 (非本正编辑) 的课程
                    if (!string.IsNullOrEmpty(model.InputName))
                    {
                        CourseInfo use = Container.Instance.Resolve<CourseInfoService>().Query(new List<ICriterion>
                    {
                        Expression.And(
                            Expression.Eq("Name", model.InputName.Trim()),
                            Expression.Not(Expression.Eq("ID", model.ID))
                        )
                    }).FirstOrDefault();
                        if (use != null)
                        {
                            return Json(new { code = -3, message = "课程已经存在，请更换课程名" });
                        }
                    }
                    dbEntry.Name = model.InputName?.Trim();

                    Container.Instance.Resolve<CourseInfoService>().Edit(dbEntry);

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
            return View();
        }

        [HttpPost]
        public JsonResult Create(CourseInfoForEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CourseInfo dbModel = (CourseInfo)model;
                    bool isCan = Container.Instance.Resolve<CourseInfoService>().CheckCourseInfo(dbModel, out string message);
                    if (!isCan)
                    {
                        return Json(new { code = -3, message = message });
                    }

                    dbModel.Name = model.InputName?.Trim();

                    Container.Instance.Resolve<CourseInfoService>().Create(dbModel);

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