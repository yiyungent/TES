using Domain;
using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Infrastructure.Abstract
{
    public interface IAuthManager
    {
        bool AuthPass(UserInfo userInfo, string authKey);

        bool AuthPass(string authKey);

        bool HasAuth(UserInfo userInfo, string authKey);

        bool HasAuth(UserInfo userInfo, string areaName, string controllerName, string actionName);

        bool HasAuth(UserInfo userInfo, string controllerName, string actionName);

        bool HasAuth(string authKey);

        bool HasAuth(string areaName, string controllerName, string actionName);

        bool HasAuth(string controllerName, string actionName);

        string GetAuthNameByKey(string authKey);

        string GetAuthKey(string areaName, string controllerName, string actionName);

        string GetAuthKey(string controllerName, string actionName);

        bool NeedAuth(string authKey);

        IList<AreaCAItem> AllAreaCAItem();

        IList<string> AllAuthKey();

        IList<Sys_Menu> GetMenuListByAccount(UserInfo userInfo);
    }
}
