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

        UserInfo GetUserInfoByTokenCookieKey(string tokenCookieValue);

        IList<Sys_Menu> AllMenuList();

        IList<FunctionInfo> AllFuncList();
    }
}
