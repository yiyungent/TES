using Core;
using Domain;
using Domain.Base;
using Framework.Common;
using Framework.Config;
using Framework.Infrastructure.Abstract;
using Service;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Infrastructure.Concrete
{
    public class AuthManager : IAuthManager
    {
        public IDBAccessProvider _dBAccessProvider;

        public AuthManager(IDBAccessProvider dBAccessProvider)
        {
            this._dBAccessProvider = dBAccessProvider;
        }

        public string GetAuthNameByKey(string authKey)
        {
            string authName = null;

            #region 废弃
            //// 此处耦合--等待数据库操作接口
            //FunctionInfo func = Container.Instance.Resolve<FunctionInfoService>().Query(new List<ICriterion>
            //{
            //    Expression.Eq("AuthKey", authKey)
            //}).FirstOrDefault(); 
            #endregion

            FunctionInfo func = _dBAccessProvider.GetFunctionInfoByAuthKey(authKey);

            if (func != null)
            {
                authName = func.Name;
            }

            return authName;
        }

        public bool HasAuth(UserInfo userInfo, string authKey)
        {
            if (userInfo == null || userInfo.RoleInfoList == null || userInfo.RoleInfoList.Count == 0)
            {
                return false;
            }
            foreach (RoleInfo role in userInfo.RoleInfoList)
            {
                if (role.FunctionInfoList == null || role.FunctionInfoList.Count == 0)
                {
                    continue;
                }
                // 判断是否具有此具体操作权限
                foreach (FunctionInfo func in role.FunctionInfoList)
                {
                    if (func.AuthKey == authKey)
                    {
                        return true;
                    }
                }
            }

            return false; ;
        }

        public bool HasAuth(string authKey)
        {
            // 获取当前登录用户
            UserInfo userInfo = Tools.GetSession<UserInfo>(AppConfig.LoginAccountSessionKey);
            if (userInfo == null)
            {
                // 当前未登录则为 游客
                userInfo = UserInfo_Guest.Instance;
            }

            return HasAuth(userInfo, authKey);
        }

        public bool HasAuth(UserInfo userInfo, string areaName, string controllerName, string actionName)
        {
            string authKey = GetAuthKey(areaName, controllerName, actionName);

            return HasAuth(userInfo, authKey);
        }

        public bool HasAuth(UserInfo userInfo, string controllerName, string actionName)
        {
            return HasAuth(userInfo, null, controllerName, actionName);
        }

        public bool HasAuth(string areaName, string controllerName, string actionName)
        {
            UserInfo userInfo = Tools.GetSession<UserInfo>(AppConfig.LoginAccountSessionKey);

            return HasAuth(userInfo, areaName, controllerName, actionName);
        }

        public bool HasAuth(string controllerName, string actionName)
        {
            return HasAuth("", controllerName, actionName);
        }

        public string GetAuthKey(string areaName, string controllerName, string actionName)
        {
            return areaName + "." + controllerName + "." + actionName;
        }

        public string GetAuthKey(string controllerName, string actionName)
        {
            return GetAuthKey(null, controllerName, actionName);
        }

        public bool NeedAuth(string authKey)
        {
            // 只要此 AuthKey 不存在于 FunctionInfo 则不需要权限认证
            bool needAuth = false;
            // 所有存在于 FunctionInfo 表中的操作都需要权限认证
            IList<string> needAuthAllAuthKeys = AllAuthKey();

            if (needAuthAllAuthKeys.Contains(authKey, new AuthKeyCompare()))
            {
                needAuth = true;
            }

            return needAuth;
        }

        #region 获取所有需要权限验证的操作地址
        /// <summary>
        /// 获取所有需要权限验证的操作地址
        /// </summary>
        public IList<AreaCAItem> AllAreaCAItem()
        {
            IList<AreaCAItem> rtnList = new List<AreaCAItem>();
            //IList<FunctionInfo> allFunction = Container.Instance.Resolve<FunctionInfoService>().GetAll();
            IList<FunctionInfo> allFunction = _dBAccessProvider.GetAllFunctionInfo();

            foreach (FunctionInfo func in allFunction)
            {
                rtnList.Add(func.AreaCAItem);
            }

            return rtnList;
        }
        #endregion

        #region 获取所有需要权限认证的权限(操作)键
        public IList<string> AllAuthKey()
        {
            IList<string> rtnList = new List<string>();
            //var allFunction = Container.Instance.Resolve<FunctionInfoService>().GetAll();
            IList<FunctionInfo> allFunction = _dBAccessProvider.GetAllFunctionInfo();
            rtnList = (from m in allFunction
                       select m.AuthKey.Trim()).ToList();

            return rtnList;
        }
        #endregion

        #region 认证通过--有权限/此请求不需要权限验证
        /// <summary>
        /// 认证通过--有权限/此请求不需要权限验证
        /// </summary>
        public bool AuthPass(UserInfo userInfo, string authKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 认证通过--有权限/此请求不需要权限验证
        /// </summary>
        public bool AuthPass(string authKey)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 权限(操作)键相等比较器
        sealed class AuthKeyCompare : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.ToLower() == y.ToLower();
            }

            public int GetHashCode(string obj)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region 获取此用户的系统菜单列表
        public IList<Sys_Menu> GetMenuListByAccount(UserInfo userInfo)
        {
            IList<Sys_Menu> menuList = new List<Sys_Menu>();

            foreach (RoleInfo role in userInfo.RoleInfoList)
            {
                foreach (Sys_Menu menu in role.Sys_MenuList)
                {
                    if (!menuList.Contains(menu, new Sys_Menu_Compare()))
                    {
                        menuList.Add(menu);
                    }
                }
            }

            return menuList;
        }

        sealed class Sys_Menu_Compare : IEqualityComparer<Sys_Menu>
        {
            public bool Equals(Sys_Menu x, Sys_Menu y)
            {
                if (x == null || y == null)
                {
                    return false;
                }
                return x.ID == y.ID;
            }

            public int GetHashCode(Sys_Menu obj)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

    }
}
