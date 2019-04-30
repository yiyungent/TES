using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Framework.HtmlHelpers;

namespace WebUI.Areas.Admin.Models
{
    public class UserInfoListViewModel
    {
        public IList<UserInfo> UserInfos { get; set; }

        public PageInfo PageInfo { get; set; }
    }
}