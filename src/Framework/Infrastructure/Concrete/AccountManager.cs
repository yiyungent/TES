using Domain;
using Framework.Common;
using Framework.Config;
using Framework.Factories;
using Framework.Infrastructure.Abstract;
using Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Framework.Infrastructure.Concrete
{
    public enum LoginStatus
    {
        /// <summary>
        /// 已登录
        /// </summary>
        IsLogin,

        /// <summary>
        /// 未登录
        /// </summary>
        WithoutLogin,

        /// <summary>
        /// 登录超时
        /// <para>使用"记住我", 但 已经过期</para>
        /// </summary>
        LoginTimeOut
    }

    public class AccountManager
    {
        private static string _loginAccountSessionKey = AppConfig.LoginAccountSessionKey;
        private static string _rememberMeTokenCookieKey = AppConfig.RememberMeTokenCookieKey;
        private static int _rememberMeDayCount = AppConfig.RememberMeDayCount;

        private static IDBAccessProvider _dBAccessProvider = DBAccessProviderFactory.Get();

        #region 获取当前UserInfo
        /// <summary>
        /// 获取当前 <see cref="UserInfo"/>, 若未登录，则为 <see cref="UserInfo_Guest.Instance"/>
        /// </summary>
        /// <returns></returns>
        public static UserInfo GetCurrentUserInfo()
        {
            // 获取当前登录用户
            UserInfo userInfo = Tools.GetSession<UserInfo>(AppConfig.LoginAccountSessionKey);
            if (userInfo == null)
            {
                // 当前未登录则为 游客
                userInfo = UserInfo_Guest.Instance;
            }

            return userInfo;
        }
        #endregion

        #region 获取当前账号
        public static CurrentAccountModel GetCurrentAccount()
        {
            CurrentAccountModel currentAccount = new CurrentAccountModel();
            currentAccount.UserInfo = GetCurrentUserInfo();
            switch (CheckLoginStatus())
            {
                case LoginStatus.IsLogin:
                    currentAccount.IsGuest = false;
                    break;
                case LoginStatus.WithoutLogin:
                    currentAccount.IsGuest = true;
                    break;
                case LoginStatus.LoginTimeOut:
                    currentAccount.IsGuest = true;
                    break;
                default:
                    currentAccount.IsGuest = true;
                    break;
            }

            return currentAccount;
        }
        #endregion

        #region 检查登录状态-已登录/未登录(登录超时)
        public static LoginStatus CheckLoginStatus()
        {
            LoginStatus loginStatus = LoginStatus.WithoutLogin;

            HttpRequest request = HttpContext.Current.Request;
            HttpSessionState session = HttpContext.Current.Session;

            UserInfo userInfo = Tools.GetSession<UserInfo>(AppConfig.LoginAccountSessionKey);
            if (userInfo != null)
            {
                loginStatus = LoginStatus.IsLogin;
            }
            else
            {

                #region 记住我
                if (request.Cookies.AllKeys.Contains(_rememberMeTokenCookieKey))
                {
                    if (request.Cookies[_rememberMeTokenCookieKey] != null && string.IsNullOrEmpty(request.Cookies[_rememberMeTokenCookieKey].Value) == false)
                    {
                        string cookieTokenValue = request.Cookies[_rememberMeTokenCookieKey].Value;
                        UserInfo user = _dBAccessProvider.GetUserInfoByTokenCookieKey(cookieTokenValue);

                        if (user == null)
                        {
                            // 口令不正确
                            loginStatus = LoginStatus.WithoutLogin;
                        }
                        else if (user.LastLoginTime.AddDays(_rememberMeDayCount) > DateTime.UtcNow)
                        {
                            // 最多 "记住我" 保存7天的 登录状态
                            loginStatus = LoginStatus.IsLogin;
                        }
                        else
                        {
                            // 登录 已过期
                            loginStatus = LoginStatus.LoginTimeOut;
                        }
                    }
                }
                #endregion

            }

            return loginStatus;
        }
        #endregion
    }
}
