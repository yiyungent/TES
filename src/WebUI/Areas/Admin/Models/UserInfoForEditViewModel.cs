﻿using Core;
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
using System.Web.Mvc;
using WebUI.Extensions;

namespace WebUI.Areas.Admin.Models
{
    public class UserInfoForEditViewModel
    {
        #region Properties

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

        [Display(Name = "角色")]
        public List<OptionModel> RoleOptions { get; set; } 

        #endregion

        #region Ctor
        public UserInfoForEditViewModel()
        {
            this.RoleOptions = new List<OptionModel>();
            //this.RoleOptions.Add(new OptionModel
            //{
            //    ID = 0,
            //    IsSelected = true,
            //    Text = "请选择角色"
            //});
            IList<RoleInfo> allRole = Container.Instance.Resolve<RoleInfoService>().GetAll();
            allRole = allRole.Where(m => m.Name != "游客").ToList();
            foreach (RoleInfo item in allRole)
            {
                this.RoleOptions.Add(new OptionModel
                {
                    ID = item.ID,
                    IsSelected = false,
                    Text = item.Name
                });
            }
        }
        #endregion

        #region 数据库模型->视图模型
        public static explicit operator UserInfoForEditViewModel(UserInfo dbModel)
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
                    IsSelected = dbModel.RoleInfoList.Contains(role, new RoleInfoEqualityComparer())
                });
            }
            UserInfoForEditViewModel viewModel = new UserInfoForEditViewModel
            {
                ID = dbModel.ID,
                InputUserName = dbModel.UserName,
                InputName = dbModel.Name,
                InputAvatar = dbModel.Avatar,
                InputEmail = dbModel.Email,
                RoleOptions = roleOptions,
                InputEmployeeCode = dbModel.GetBindEmployee()?.EmployeeCode ?? "",
                InputStudentCode = dbModel.GetBindStudent()?.StudentCode ?? ""
            };

            return viewModel;
        }
        #endregion

        #region 输入模型->数据库模型
        public static explicit operator UserInfo(UserInfoForEditViewModel inputModel)
        {
            UserInfo dbModel = null;
            if (inputModel.ID == 0)
            {
                // 创建
                dbModel = new UserInfo();
                dbModel.RegTime = DateTime.Now;
            }
            else
            {
                // 修改
                dbModel = Container.Instance.Resolve<UserInfoService>().GetEntity(inputModel.ID);
            }
            #region 赋值转换
            if (!string.IsNullOrEmpty(inputModel.InputPassword))
            {
                dbModel.Password = EncryptHelper.MD5Encrypt32(inputModel.InputPassword);
            }
            #region 角色选项
            if (inputModel.RoleOptions != null)
            {
                IList<int> roleIdList = new List<int>();
                foreach (OptionModel option in inputModel.RoleOptions)
                {
                    roleIdList.Add(option.ID);
                }
                IList<RoleInfo> selectedRole = Container.Instance.Resolve<RoleInfoService>().Query(new List<ICriterion>
                {
                    Expression.In("ID", roleIdList.ToArray())
                });
                dbModel.RoleInfoList = selectedRole;
            }
            else
            {
                dbModel.RoleInfoList = null;
            }
            #endregion
            #region 绑定学号，工号
            if (!string.IsNullOrEmpty(inputModel.InputStudentCode?.Trim()))
            {
                // 为此学生绑定上此用户
                dbModel.BindStudent(inputModel.InputStudentCode?.Trim());
            }
            else
            {
                if (inputModel.ID != 0)
                {
                    dbModel.UnBindStudent();
                }
            }
            if (!string.IsNullOrEmpty(inputModel.InputEmployeeCode?.Trim()))
            {
                // 为此员工绑定上此用户
                dbModel.BindEmployee(inputModel.InputEmployeeCode?.Trim());
            }
            else
            {
                if (inputModel.ID != 0)
                {
                    dbModel.UnBindEmployee();
                }
            }
            #endregion
            dbModel.UserName = inputModel.InputUserName?.Trim();
            dbModel.Name = inputModel.InputName?.Trim();
            dbModel.Avatar = inputModel.InputAvatar?.Trim();
            dbModel.Email = inputModel.InputEmail?.Trim();
            dbModel.Description = inputModel.InputDescription?.Trim();
            #endregion

            return dbModel;
        }
        #endregion
    }
}