using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Account.Models
{
    public class EditUserInfoModel
    {
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "用户名不能为空")]
        public string InputName { get; set; }

        [Display(Name = "账号")]
        [Required(ErrorMessage = "账号不能为空")]
        public string InputAccount { get; set; }

        [Display(Name = "邮箱")]
        public string InputEmail { get; set; }

        [Display(Name = "描述")]
        public string InputDescription { get; set; }
    }
}