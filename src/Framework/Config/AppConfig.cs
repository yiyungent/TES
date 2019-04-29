using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Config
{
    public class AppConfig
    {
        public static string LoginAccountSessionKey { get; set; } = "LoginAccount";

        public static string RememberMeTokenCookieKey { get; set; } = "Token";

        public static int RememberMeDayCount { get; set; } = 7;

        public static string LoginPageUrl { get; set; } = "";
    }
}
