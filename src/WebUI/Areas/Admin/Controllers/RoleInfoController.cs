using Core;
using Domain;
using Framework.Factories;
using Framework.HtmlHelpers;
using Framework.Infrastructure.Abstract;
using Framework.Infrastructure.Concrete;
using Framework.Models;
using Framework.Mvc;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Admin.Controllers
{
    public class RoleInfoController : BaseController
    {

        #region Ctor
        public RoleInfoController()
        {
            ViewBag.PageHeader = "角色管理";
            ViewBag.PageHeaderDescription = "角色管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
            };
        }
        #endregion

        #region 首页-列表
        public ViewResult Index(CurrentAccountModel currentAccount, int pageIndex = 1, int pageSize = 6)
        {
            IList<RoleInfo> list = Container.Instance.Resolve<RoleInfoService>().GetAll();
            // 当前页号超过总页数，则显示最后一页
            int lastPageIndex = (int)Math.Ceiling((double)list.Count / pageSize);
            pageIndex = pageIndex <= lastPageIndex ? pageIndex : lastPageIndex;

            // 使用 Skip 还顺便解决了 若 pageIndex <= 0 的错误情况
            var data = (from m in list
                        orderby m.ID descending
                        select m).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            RoleInfoListViewModel model = new RoleInfoListViewModel
            {
                RoleInfos = data.ToList(),
                PageInfo = new PageInfo
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecordCount = list.Count,
                    MaxLinkCount = 10
                }
            };

            return View(model);
        }
        #endregion

        #region 删除
        public JsonResult Delete(int id)
        {
            try
            {
                Container.Instance.Resolve<RoleInfoService>().Delete(id);

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
            RoleInfo model = Container.Instance.Resolve<RoleInfoService>().GetEntity(id);

            return View(model);
        }
        #endregion

        #region 编辑
        [HttpGet]
        public ViewResult Edit(int id)
        {
            RoleInfo model = Container.Instance.Resolve<RoleInfoService>().GetEntity(id);

            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(RoleInfoForEdit model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RoleInfo dbEntry = Container.Instance.Resolve<RoleInfoService>().GetEntity(model.ID);
                    dbEntry.Name = model.Name;

                    Container.Instance.Resolve<RoleInfoService>().Edit(dbEntry);

                    return Json(new { code = 1, message = "保存成功" });
                }
                else
                {
                    return Json(new { code = 1, message = "不合理的输入" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "保存失败" });
            }
        }
        #endregion

        #region 授权
        [HttpGet]
        public ViewResult AssignPower(int id)
        {
            RoleInfo model = Container.Instance.Resolve<RoleInfoService>().GetEntity(id);

            return View(model);
        }

        [HttpPost]
        public JsonResult AssignPower(string model)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    return Json(new { code = 1, message = "保存成功" });
                }
                else
                {
                    return Json(new { code = 1, message = "不合理的输入" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "保存失败" });
            }
        }
        #endregion

    }
}