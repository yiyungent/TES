using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models
{
    public class UserInfoForEdit
    {
        public int ID { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string InputAccount { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string InputName { get; set; }

        /// <summary>
        /// 用户头像Url地址
        /// </summary>
        public string InputAvatar { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string InputEmail { get; set; }


        public List<RoleOption> RoleOptions { get; set; }
    }

    public class RoleOption
    {
        public int ID { get; set; }

        public string Text { get; set; }

        public bool IsSelected { get; set; }
    }
}