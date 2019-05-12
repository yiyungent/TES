using Core;
using Domain;
using Framework.Factories;
using Framework.HtmlHelpers;
using Framework.Infrastructure.Abstract;
using Framework.Infrastructure.Concrete;
using Framework.Models;
using Framework.Mvc;
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
    public class UserInfoController : BaseController
    {

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
            TempData["RedirectUrl"] = Request.RawUrl;

            #region MyRegion
            //ViewBag.LoginAccount = currentAccount.UserInfo;
            //ViewBag.MenuList = this.AuthManager.GetMenuListByUserInfo(currentAccount.UserInfo);
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
            UserInfo userInfo = Container.Instance.Resolve<UserInfoService>().GetEntity(id);
            IList<RoleInfo> allRole = Container.Instance.Resolve<RoleInfoService>().GetAll();
            allRole = allRole.Where(m => m.Name != "游客").ToList();

            List<RoleOption> roleOptions = new List<RoleOption>();
            foreach (RoleInfo role in allRole)
            {
                roleOptions.Add(new RoleOption
                {
                    ID = role.ID,
                    Text = role.Name,
                    IsSelected = userInfo.RoleInfoList.Contains(role, new RoleInfoEqualityComparer())
                });
            }
            UserInfoForEditViewModel model = new UserInfoForEditViewModel
            {
                ID = userInfo.ID,
                InputUserName = userInfo.UserName,
                InputName = userInfo.Name,
                InputAvatar = userInfo.Avatar,
                InputEmail = userInfo.Email,
                RoleOptions = roleOptions
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(UserInfoForEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserInfo dbEntry = Container.Instance.Resolve<UserInfoService>().GetEntity(model.ID);
                    dbEntry.Name = model.InputName?.Trim();
                    //dbEntry.Avatar = model.Avatar;

                    // 查找 已经绑定此邮箱的 (非本正编辑) 的用户
                    if (!string.IsNullOrEmpty(model.InputEmail))
                    {
                        UserInfo useNeedEmailUser = Container.Instance.Resolve<UserInfoService>().Query(new List<ICriterion>
                    {
                        Expression.And(
                            Expression.Eq("Email", model.InputEmail.Trim()),
                            Expression.Not(Expression.Eq("ID", model.ID))
                        )
                    }).FirstOrDefault();
                        if (useNeedEmailUser != null)
                        {
                            return Json(new { code = -3, message = "邮箱已经被其他用户绑定，请绑定其他邮箱" });
                        }
                    }
                    dbEntry.Email = model.InputEmail?.Trim();


                    IList<int> roleIdList = new List<int>();
                    foreach (RoleOption option in model.RoleOptions)
                    {
                        roleIdList.Add(option.ID);
                    }
                    IList<RoleInfo> selectedRole = Container.Instance.Resolve<RoleInfoService>().Query(new List<ICriterion>
                    {
                        Expression.In("ID", roleIdList.ToArray())
                    });
                    dbEntry.RoleInfoList = selectedRole;

                    Container.Instance.Resolve<UserInfoService>().Edit(dbEntry);

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