using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models
{
    public class RoleInfoForEdit
    {
        public int ID { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { get; set; }
    }
}