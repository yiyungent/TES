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
using WebUI.Areas.Admin.Models.Common;
using WebUI.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    public class UserInfoController : Controller
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
        public ViewResult Index(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            Query(queryConditions);

            ListViewModel<UserInfo> model = new ListViewModel<UserInfo>(queryConditions, pageIndex: pageIndex, pageSize: pageSize);
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(model);
        }

        private void Query(IList<ICriterion> queryConditions)
        {
            // 输入的查询关键词
            string query = Request["q"]?.Trim() ?? "";
            // 查询类型
            QueryType queryType = new QueryType();
            queryType.Val = Request["type"]?.Trim() ?? "username";
            switch (queryType.Val.ToLower())
            {
                case "username":
                    queryType.Text = "用户名";
                    queryConditions.Add(Expression.Like("UserName", query, MatchMode.Anywhere));
                    break;
                case "name":
                    queryType.Text = "展示名";
                    queryConditions.Add(Expression.Like("Name", query, MatchMode.Anywhere));
                    break;
                case "id":
                    queryType.Text = "ID";
                    queryConditions.Add(Expression.Eq("ID", int.Parse(query)));
                    break;
                default:
                    queryType.Text = "用户名";
                    queryConditions.Add(Expression.Like("UserName", query, MatchMode.Anywhere));
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
            UserInfoForEditViewModel model = (UserInfoForEditViewModel)userInfo;

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
                        if (IsExistEmail(model.InputEmail, model.ID))
                        {
                            return Json(new { code = -3, message = "邮箱已经被其他用户绑定，请绑定其他邮箱" });
                        }
                    }
                    dbEntry.Email = model.InputEmail?.Trim();
                    #region 绑定与解绑员工号
                    if (!string.IsNullOrEmpty(model.InputEmployeeCode?.Trim()))
                    {
                        // 有输入 EmployeeCode
                        // 验证是否已经被其它用户绑定
                        EmployeeInfo use = Container.Instance.Resolve<EmployeeInfoService>().Query(new List<ICriterion>
                        {
                            Expression.And(
                                Expression.Eq("EmployeeCode", model.InputEmployeeCode),
                                Expression.Not(Expression.Eq("UID", model.ID))
                            )
                        }).FirstOrDefault();
                        if (use != null)
                        {
                            return Json(new { code = -3, message = "工号已经被其它用户绑定，请更换" });
                        }
                        else
                        {
                            if (!Container.Instance.Resolve<EmployeeInfoService>().Exists(model.InputEmployeeCode))
                            {
                                return Json(new { code = -4, message = "工号不存在，请更换" });
                            }
                            else
                            {
                                dbEntry.BindEmployee(model.InputEmployeeCode.Trim());
                            }
                        }
                    }
                    else
                    {
                        // 未输入 EmployeeCode - 解绑
                        dbEntry.UnBindEmployee();
                    }
                    #endregion
                    #region 绑定与解绑学号
                    if (!string.IsNullOrEmpty(model.InputStudentCode?.Trim()))
                    {
                        // 有输入 StudentCode
                        // 验证是否已经被其它用户绑定
                        StudentInfo use = Container.Instance.Resolve<StudentInfoService>().Query(new List<ICriterion>
                        {
                            Expression.And(
                                Expression.Eq("StudentCode", model.InputStudentCode),
                                Expression.Not(Expression.Eq("UID", model.ID))
                            )
                        }).FirstOrDefault();
                        if (use != null)
                        {
                            return Json(new { code = -3, message = "学号已经被其它用户绑定，请更换" });
                        }
                        else
                        {
                            if (!Container.Instance.Resolve<StudentInfoService>().Exists(model.InputStudentCode))
                            {
                                return Json(new { code = -4, message = "学号不存在，请更换" });
                            }
                            else
                            {
                                dbEntry.BindEmployee(model.InputStudentCode.Trim());
                            }
                        }
                    }
                    else
                    {
                        // 未输入 EmployeeCode - 解绑
                        dbEntry.UnBindEmployee();
                    }
                    #endregion

                    UserInfo userInfo = (UserInfo)model;
                    dbEntry.RoleInfoList = userInfo.RoleInfoList;

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

        #region 新增
        [HttpGet]
        public ViewResult Create()
        {
            UserInfoForEditViewModel model = new UserInfoForEditViewModel()
            {
                RoleOptions = new List<OptionModel>()
            };
            IList<RoleInfo> allRole = Container.Instance.Resolve<RoleInfoService>().GetAll();
            foreach (RoleInfo item in allRole)
            {
                model.RoleOptions.Add(new OptionModel
                {
                    ID = item.ID,
                    IsSelected = false,
                    Text = item.Name
                });
            }
            //model.RoleOptions.Insert(0, new RoleOption
            //{
            //    ID = 0,
            //    IsSelected = true,
            //    Text = "请选择角色"
            //});

            return View(model);
        }

        [HttpPost]
        public JsonResult Create(UserInfoForEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserInfo dbModel = (UserInfo)model;
                    // 查找 已经有此用户名的用户
                    if (!string.IsNullOrEmpty(model.InputUserName))
                    {
                        UserInfo use = Container.Instance.Resolve<UserInfoService>().Query(new List<ICriterion>
                        {
                            Expression.Eq("UserName", model.InputUserName.Trim())
                        }).FirstOrDefault();
                        if (use != null)
                        {
                            return Json(new { code = -3, message = "用户名已有，请使用其他用户名" });
                        }
                    }
                    // 查找 已经绑定此邮箱的 (非本正编辑) 的用户
                    if (!string.IsNullOrEmpty(model.InputEmail))
                    {
                        UserInfo use = Container.Instance.Resolve<UserInfoService>().Query(new List<ICriterion>
                    {
                            Expression.Eq("Email", model.InputEmail.Trim()),
                    }).FirstOrDefault();
                        if (use != null)
                        {
                            return Json(new { code = -3, message = "邮箱已经被其他用户绑定，请绑定其他邮箱" });
                        }
                    }

                    Container.Instance.Resolve<UserInfoService>().Create(dbModel);

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

        #region Helpers

        #region 检查邮箱是否已(被其它用户)绑定
        public bool IsExistEmail(string email, int userId = 0)
        {
            bool isExist = false;
            IList<ICriterion> criteria = null;
            if (userId == 0)
            {
                criteria = new List<ICriterion>
                {
                    Expression.Eq("Email", email)
                };
            }
            else
            {
                criteria = new List<ICriterion>
                {
                     Expression.And(
                        Expression.Eq("Email", email),
                        Expression.Not(Expression.Eq("ID", userId))
                     )
                };
            }
            UserInfo exist = Container.Instance.Resolve<UserInfoService>().Query(criteria).FirstOrDefault();
            if (exist != null)
            {
                isExist = true;
            }

            return isExist;
        }
        #endregion

        #endregion

    }

}