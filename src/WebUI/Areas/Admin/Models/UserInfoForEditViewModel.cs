using Core;
using Domain;
using Framework.Infrastructure.Concrete;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models
{
    public class UserInfoForEditViewModel
    {
        /// <summary>
        /// UID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "用户名不能为空")]
        public string InputUserName { get; set; }

        /// <summary>
        /// 展示名
        /// </summary>
        [Display(Name = "展示名")]
        [Required(ErrorMessage = "展示名不能为空")]
        public string InputName { get; set; }

        /// <summary>
        /// 用户头像Url地址
        /// </summary>
        public string InputAvatar { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Display(Name = "邮箱")]
        public string InputEmail { get; set; }

        [Display(Name = "描述")]
        public string InputDescription { get; set; }

        public List<RoleOption> RoleOptions { get; set; }

        public static explicit operator UserInfoForEditViewModel(UserInfo userInfo)
        {
            IList<RoleInfo> allRole = Container.Instance.Resolve<RoleInfoService>().GetAll();
            allRole = allRole.Where(m => m.Name != "游客").ToList();

            List<RoleOption> roleOptions = new List<RoleOption>();
            foreach (RoleInfo role in allRole)
            {
                roleOptions.Add(new RoleOption
                {
                    ID = role.ID,
                    Text = role.Name,
                    IsSelected = userInfo.RoleInfoList.Contains(role, new RoleInfoEqualityComparer())
                });
            }
            UserInfoForEditViewModel rtnModel = new UserInfoForEditViewModel
            {
                ID = userInfo.ID,
                InputUserName = userInfo.UserName,
                InputName = userInfo.Name,
                InputAvatar = userInfo.Avatar,
                InputEmail = userInfo.Email,
                RoleOptions = roleOptions
            };

            return rtnModel;
        }
    }

    public class RoleOption
    {
        public int ID { get; set; }

        public string Text { get; set; }

        public bool IsSelected { get; set; }
    }
}