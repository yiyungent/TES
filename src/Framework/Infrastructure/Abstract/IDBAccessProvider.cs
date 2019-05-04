using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Infrastructure.Abstract
{
    public interface IDBAccessProvider
    {
        FunctionInfo GetFunctionInfoByAuthKey(string authKey);

        IList<FunctionInfo> GetAllFunctionInfo();

        RoleInfo GetGuestRoleInfo();

        UserInfo GetUserInfoById(int id);

        UserInfo GetUserInfoByTokenCookieKey(string tokenCookieValue);

        IList<Sys_Menu> AllMenuList();

        IList<FunctionInfo> AllFuncList();

        bool EditRoleInfo(RoleInfo roleInfo);

        RoleInfo GetRoleInfoById(int id);

        Sys_Menu GetSys_MenuById(int id);

        FunctionInfo GetFunctionInfoById(int id);

        IList<Sys_Menu> GetSys_MenuListByIds(params int[] ids);

        IList<FunctionInfo> GetFunctionInfoListByIds(params int[] ids);

        UserInfo GetUserInfoByLoginAccount(string loginAccount);
    }
}
