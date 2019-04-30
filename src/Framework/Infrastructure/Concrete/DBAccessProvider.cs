using Core;
using Domain;
using Framework.Infrastructure.Abstract;
using Service;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Framework.Config;

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

        public UserInfo GetUserInfoByTokenCookieKey(string tokenCookieValue)
        {
            UserInfo user = Container.Instance.Resolve<UserInfoService>().Query(new List<ICriterion>
            {
                Expression.Eq(AppConfig.RememberMeTokenCookieKey, tokenCookieValue)
            }).FirstOrDefault();

            return user;
        }
    }
}