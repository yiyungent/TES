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
            };
        }
        #endregion

        #region 列表
        public ActionResult Index(CurrentAccountModel currentAccount, int pageIndex = 1, int pageSize = 6)
        {
            IList<CourseTable> list = Container.Instance.Resolve<CourseTableService>().GetAll();
            // 当前页号超过总页数，则显示最后一页
            int lastPageIndex = (int)Math.Ceiling((double)list.Count / pageSize);
            pageIndex = pageIndex <= lastPageIndex ? pageIndex : lastPageIndex;

            var data = (from m in list
                        orderby m.ID descending
                        select m).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            CourseTableListViewModel model = new CourseTableListViewModel
            {
                CourseTables = data.ToList(),
                PageInfo = new PageInfo
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecordCount = list.Count,
                    MaxLinkCount = 10
                }
            };
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(model);
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
                    CourseTable dbEntry = Container.Instance.Resolve<CourseTableService>().GetEntity(model.ID);
                    // continue...

                    Container.Instance.Resolve<CourseTableService>().Edit(dbEntry);

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

    }
}