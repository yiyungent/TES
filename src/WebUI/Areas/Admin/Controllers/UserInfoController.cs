using Core;
using Domain;
using Framework.Factories;
using Framework.HtmlHelpers;
using Framework.Infrastructure.Abstract;
using Framework.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Admin.Controllers
{
    public class UserInfoController : Controller
    {
        IAuthManager _authManager = AuthManagerFactory.Get();

        #region Ctor
        public UserInfoController()
        {
            ViewBag.PageHeader = "用户管理";
            ViewBag.PageHeaderDescription = "用户管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
            };

        }
        #endregion

        #region 首页-列表
        public ViewResult Index(CurrentAccountModel currentAccount, int pageIndex = 1, int pageSize = 6)
        {
            IList<UserInfo> list = Container.Instance.Resolve<UserInfoService>().GetAll();
            // 当前页号超过总页数，则显示最后一页
            int lastPageIndex = (int)Math.Ceiling((double)list.Count / pageSize);
            pageIndex = pageIndex <= lastPageIndex ? pageIndex : lastPageIndex;

            // 使用 Skip 还顺便解决了 若 pageIndex <= 0 的错误情况
            var data = (from m in list
                        orderby m.ID descending
                        select m).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            UserInfoListViewModel model = new UserInfoListViewModel
            {
                UserInfos = data.ToList(),
                PageInfo = new PageInfo
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecordCount = list.Count,
                    MaxLinkCount = 10
                }
            };

            #region MyRegion
            ViewBag.LoginAccount = currentAccount.UserInfo;
            ViewBag.MenuList = _authManager.GetMenuListByAccount(currentAccount.UserInfo);
            #endregion

            return View(model);
        }
        #endregion

        #region 删除
        public JsonResult Delete(int id)
        {
            try
            {
                Container.Instance.Resolve<UserInfoService>().Delete(id);

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
            UserInfo model = Container.Instance.Resolve<UserInfoService>().GetEntity(id);

            return View(model);
        }
        #endregion

        #region 编辑
        [HttpGet]
        public ViewResult Edit(int id)
        {
            UserInfo model = Container.Instance.Resolve<UserInfoService>().GetEntity(id);

            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(UserInfoForEdit model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserInfo dbEntry = Container.Instance.Resolve<UserInfoService>().GetEntity(model.ID);
                    dbEntry.Name = model.Name;
                    //dbEntry.Avatar = model.Avatar;
                    //dbEntry.Email = model.Email;
                    Container.Instance.Resolve<UserInfoService>().Edit(dbEntry);

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