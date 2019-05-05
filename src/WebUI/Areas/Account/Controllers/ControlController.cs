using Core;
using Domain;
using Framework.Infrastructure.Concrete;
using Framework.Mvc;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Areas.Account.Controllers
{
    public class ControlController : BaseController
    {

        #region loadProfileBox
        public PartialViewResult ProfileBoxPartial(string loginAccount)
        {
            UserInfo model = AccountManager.GetUserInfoByLoginAccount(loginAccount);

            return PartialView("_ProfileBoxPartial", model);
        }
        #endregion


    }
}