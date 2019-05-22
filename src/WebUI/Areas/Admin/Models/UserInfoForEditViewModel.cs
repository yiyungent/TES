using Core;
using Domain;
using Framework.Common;
using Framework.Infrastructure.Concrete;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebUI.Extensions;

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

        [Display(Name = "密码")]
        public string InputPassword { get; set; }

        [Display(Name = "绑定员工工号")]
        public string InputEmployeeCode { get; set; }

        [Display(Name = "绑定学生学号")]
        public string InputStudentCode { get; set; }

        public List<OptionModel> RoleOptions { get; set; }

        public static explicit operator UserInfoForEditViewModel(UserInfo userInfo)
        {
            IList<RoleInfo> allRole = Container.Instance.Resolve<RoleInfoService>().GetAll();
            allRole = allRole.Where(m => m.Name != "游客").ToList();

            List<OptionModel> roleOptions = new List<OptionModel>();
            foreach (RoleInfo role in allRole)
            {
                roleOptions.Add(new OptionModel
                {
                    ID = role.ID,
                    Text = role.Name,
                    IsSelected = userInfo.RoleInfoList.Contains(role, new RoleInfoEqualityComparer())
                });
            }
            UserInfoForEditViewModel rtn = new UserInfoForEditViewModel
            {
                ID = userInfo.ID,
                InputUserName = userInfo.UserName,
                InputName = userInfo.Name,
                InputAvatar = userInfo.Avatar,
                InputEmail = userInfo.Email,
                RoleOptions = roleOptions,
                InputEmployeeCode = userInfo.GetBindEmployee()?.EmployeeCode ?? "",
                InputStudentCode = userInfo.GetBindStudent()?.StudentCode ?? ""
            };

            return rtn;
        }

        public static explicit operator UserInfo(UserInfoForEditViewModel model)
        {
            UserInfo rtn = new UserInfo
            {
                ID = model.ID,
                UserName = model.InputUserName,
                Name = model.InputName,
                Avatar = model.InputAvatar,
                Email = model.InputEmail,
                Description = model.InputDescription
            };
            if (!string.IsNullOrEmpty(model.InputPassword))
            {
                rtn.Password = EncryptHelper.MD5Encrypt32(model.InputPassword);
            }
            if (model.RoleOptions != null)
            {
                IList<int> roleIdList = new List<int>();
                foreach (OptionModel option in model.RoleOptions)
                {
                    roleIdList.Add(option.ID);
                }
                IList<RoleInfo> selectedRole = Container.Instance.Resolve<RoleInfoService>().Query(new List<ICriterion>
                {
                    Expression.In("ID", roleIdList.ToArray())
                });
                rtn.RoleInfoList = selectedRole;
            }
            else
            {
                rtn.RoleInfoList = null;
            }

            if (!string.IsNullOrEmpty(model.InputStudentCode))
            {
                // 为此学生绑定上此用户
                rtn.BindStudent(model.InputStudentCode);
            }
            else
            {
                rtn.UnBindStudent();
            }
            if (!string.IsNullOrEmpty(model.InputEmployeeCode))
            {
                // 为此员工绑定上此用户
                rtn.BindEmployee(model.InputEmployeeCode);
            }
            else
            {
                rtn.UnBindEmployee();
            }

            return rtn;
        }
    }
}