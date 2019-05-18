using Domain;
using Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.MessageHubs;

namespace WebUI.Controllers
{
    public class NotificationDemoController : BaseController
    {
        public void Test(string message)
        {
            NotificationManager notificationManager = new NotificationManager();
            if (!CurrentAccount.IsGuest)
            {
                UserInfo currentUser = CurrentAccount.UserInfo;
                notificationManager.Push(currentUser.ID, "测试推送", "测试推送内容:" + message);
            }
            else
            {
                notificationManager.Push(1, "未登录推送", "未登录推送:" + message);
            }
        }
    }
}