﻿using Core;
using Domain;
using Framework.Infrastructure.Abstract;
using Service;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Framework.Config;
using Framework.Models;

namespace Framework.Infrastructure.Concrete
{
    public class DBAccessProvider : IDBAccessProvider
    {
        public IList<FunctionInfo> GetAllFunctionInfo()
        {
            IList<FunctionInfo> allFunction = Container.Instance.Resolve<FunctionInfoService>().GetAll();

            return allFunction;
        }

        public FunctionInfo GetFunctionInfoByAuthKey(string authKey)
        {
            FunctionInfo func = Container.Instance.Resolve<FunctionInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq("AuthKey", authKey)
            }).FirstOrDefault();

            return func;
        }

        public RoleInfo GetGuestRoleInfo()
        {
            return Container.Instance.Resolve<RoleInfoService>().GetEntity(2);
        }

        public UserInfo GetUserInfoById(int id)
        {
            return Container.Instance.Resolve<UserInfoService>().GetEntity(id);
        }

        public UserInfo GetUserInfoByTokenCookieKey(string tokenCookieValue)
        {
            UserInfo user = Container.Instance.Resolve<UserInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq(AppConfig.TokenCookieKey, tokenCookieValue)
            }).FirstOrDefault();

            return user;
        }


        #region 获取所有菜单
        public IList<Sys_Menu> AllMenuList()
        {
            IList<Sys_Menu> menuList = new List<Sys_Menu>();
            menuList = Container.Instance.Resolve<Sys_MenuService>().GetAll();

            return menuList;
        }
        #endregion

        #region 获取所有操作
        public IList<FunctionInfo> AllFuncList()
        {
            IList<FunctionInfo> funcList = new List<FunctionInfo>();
            funcList = Container.Instance.Resolve<FunctionInfoService>().GetAll();

            return funcList;
        }
        #endregion

        #region 编辑角色
        public bool EditRoleInfo(RoleInfo roleInfo)
        {
            bool isSuccess = false;
            try
            {
                Container.Instance.Resolve<RoleInfoService>().Edit(roleInfo);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            return isSuccess;
        }
        #endregion

        #region 编辑用户
        public bool EditUserInfo(UserInfo userInfo)
        {
            bool isSuccess = false;
            try
            {
                Container.Instance.Resolve<UserInfoService>().Edit(userInfo);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            return isSuccess;
        }
        #endregion

        public RoleInfo GetRoleInfoById(int id)
        {
            RoleInfo rtn = null;
            rtn = Container.Instance.Resolve<RoleInfoService>().GetEntity(id);

            return rtn;
        }

        public Sys_Menu GetSys_MenuById(int id)
        {
            Sys_Menu rtn = null;
            rtn = Container.Instance.Resolve<Sys_MenuService>().GetEntity(id);

            return rtn;
        }

        public FunctionInfo GetFunctionInfoById(int id)
        {
            FunctionInfo rtn = null;
            rtn = Container.Instance.Resolve<FunctionInfoService>().GetEntity(id);

            return rtn;
        }

        public IList<Sys_Menu> GetSys_MenuListByIds(params int[] ids)
        {
            IList<Sys_Menu> rtn = new List<Sys_Menu>();
            rtn = Container.Instance.Resolve<Sys_MenuService>().Query(new List<ICriterion>
            {
                Expression.In("ID", ids.ToArray())
            });

            return rtn;
        }

        public IList<FunctionInfo> GetFunctionInfoListByIds(params int[] ids)
        {
            IList<FunctionInfo> rtn = new List<FunctionInfo>();
            rtn = Container.Instance.Resolve<FunctionInfoService>().Query(new List<ICriterion>
            {
                Expression.In("ID", ids.ToArray())
            });

            return rtn;
        }

        public UserInfo GetUserInfoByUserName(string userName)
        {
            UserInfo rtn = null;
            rtn = Container.Instance.Resolve<UserInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq("UserName", userName)
            }).FirstOrDefault();

            return rtn;
        }

        public IList<FunctionInfo> GetFunctionListBySys_MenuId(int sys_menuId)
        {
            IList<FunctionInfo> rtn = null;
            rtn = Container.Instance.Resolve<FunctionInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq("Sys_Menu.ID", sys_menuId)
            });

            return rtn;
        }

        public string GetSet(string key)
        {
            string rtn = null;
            rtn = Container.Instance.Resolve<SettingService>().GetSet(key);

            return rtn;
        }

        public void SaveUserTemplateName(string templateName)
        {
            CurrentAccountModel currentAccount = AccountManager.GetCurrentAccount();
            if (!currentAccount.IsGuest)
            {
                currentAccount.UserInfo.TemplateName = templateName;
                Container.Instance.Resolve<UserInfoService>().Edit(currentAccount.UserInfo);
            }
        }
    }
}