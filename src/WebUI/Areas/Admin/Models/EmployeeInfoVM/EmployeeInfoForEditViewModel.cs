using Core;
using Domain;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Areas.Admin.Models.EmployeeInfoVM
{
    public class EmployeeInfoForEditViewModel
    {
        [Display(Name = "编号")]
        public int ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        [Required]
        public string InputName { get; set; }

        /// <summary>
        /// 工号（注册时，此作为 绑定用户的 LoginAccount）
        /// </summary>
        [Display(Name = "工号")]
        [Required]
        public string InputEmployeeCode { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        [Display(Name = "入职时间")]
        [Required]
        public DateTime InputStart_Time { get; set; }

        /// <summary>
        /// 离职时间
        /// </summary>
        [Display(Name = "离职时间")]
        [Required]
        public DateTime InputEnd_Time { get; set; }

        ///<summary>
        /// 仅作展示，选择职位
        /// </summary>
        [Display(Name = "职位")]
        public IList<SelectListItem> SelectListForDuty { get; set; }

        /// <summary>
        /// 仅作接收, 被选中的职位
        /// </summary>
        public int SelectedValForDuty { get; set; }

        ///<summary>
        /// 仅作展示，选择性别
        /// </summary>
        [Display(Name = "性别")]
        public IList<SelectListItem> SelectListForSex { get; set; }

        /// <summary>
        /// 仅作接收, 被选中的性别
        /// </summary>
        public int SelectedValForSex { get; set; }

        ///<summary>
        /// 仅作展示，选择部门
        /// </summary>
        [Display(Name = "部门")]
        public IList<SelectListItem> SelectListForDept { get; set; }

        /// <summary>
        /// 仅作接收, 被选中的部门
        /// </summary>
        public int SelectedValForDept { get; set; }

        #region Ctor
        public EmployeeInfoForEditViewModel()
        {
            this.SelectListForDept = InitSelectListForDept(0);
            this.SelectedValForDept = 0;
            this.SelectListForSex = InitSelectListForSex(0);
            this.SelectedValForSex = 0;
            this.SelectListForDuty = InitSelectListForDuty(0);
            this.SelectedValForDuty = 0;
        }
        #endregion

        #region 数据库模型->视图模型
        public static explicit operator EmployeeInfoForEditViewModel(EmployeeInfo dbModel)
        {
            EmployeeInfoForEditViewModel viewModel = new EmployeeInfoForEditViewModel
            {
                ID = dbModel.ID,
                InputName = dbModel.Name,
                InputEmployeeCode = dbModel.EmployeeCode,
                InputStart_Time = dbModel.Start_Time,
                InputEnd_Time = dbModel.End_Time,
                SelectListForDept = InitSelectListForDept(dbModel.Department?.ID ?? 0),
                SelectedValForDept = dbModel.Department?.ID ?? 0,
                SelectListForSex = InitSelectListForSex(dbModel.Sex),
                SelectedValForSex = dbModel.Sex,
                SelectListForDuty = InitSelectListForDuty(dbModel.EmployeeDuty?.ID ?? 0),
                SelectedValForDuty = dbModel.EmployeeDuty?.ID ?? 0
            };

            return viewModel;
        }
        #endregion

        #region 输入模型->数据库模型
        public static explicit operator EmployeeInfo(EmployeeInfoForEditViewModel inputModel)
        {
            EmployeeInfo dbModel = null;
            if (inputModel.ID == 0)
            {
                // 创建
                dbModel = new EmployeeInfo();
            }
            else
            {
                // 修改
                dbModel = Container.Instance.Resolve<EmployeeInfoService>().GetEntity(inputModel.ID);
            }
            dbModel.Name = inputModel.InputName?.Trim();
            dbModel.EmployeeCode = inputModel.InputEmployeeCode;
            dbModel.Start_Time = inputModel.InputStart_Time;
            dbModel.End_Time = inputModel.InputEnd_Time;
            dbModel.Department = new Department { ID = inputModel.SelectedValForDept };
            dbModel.Sex = inputModel.SelectedValForSex;
            dbModel.EmployeeDuty = new EmployeeDuty { ID = inputModel.SelectedValForDuty };

            return dbModel;
        }
        #endregion

        #region 初始化选项列表-部门
        /// <summary>
        /// 初始化选项列表-部门
        /// </summary>
        private static IList<SelectListItem> InitSelectListForDept(int selectedValue)
        {
            IList<SelectListItem> ret = new List<SelectListItem>();
            ret.Add(new SelectListItem()
            {
                Text = "请选择",
                Value = "0",
                Selected = (selectedValue == 0)
            });
            // 所有 列表
            IList<Department> allList = Container.Instance.Resolve<DepartmentService>().GetAll();
            // 院级 列表
            IList<Department> yuanList = allList.Where(m => m.ParentDept == null || m.ParentDept.ID == 0).ToList();
            // 系级 列表
            IList<Department> xiList = allList.Where(m => m.Children == null || m.Children.Count == 0).ToList();
            foreach (var yuanItem in yuanList)
            {
                foreach (var xiItem in xiList)
                {
                    ret.Add(new SelectListItem()
                    {
                        Text = yuanItem.Name + " - " + xiItem.Name,
                        Value = xiItem.ID.ToString(),
                        Selected = (selectedValue == xiItem.ID)
                    });
                }
            }

            return ret;
        }
        #endregion 

        #region 初始化选项列表-性别
        /// <summary>
        /// 初始化选项列表-性别
        /// </summary>
        private static IList<SelectListItem> InitSelectListForSex(int selectedValue)
        {
            IList<SelectListItem> ret = new List<SelectListItem>();
            ret.Add(new SelectListItem()
            {
                Text = "请选择",
                Value = "0",
                Selected = (selectedValue == 0)
            });
            ret.Add(new SelectListItem()
            {
                Text = "男",
                Value = "1",
                Selected = (selectedValue == 1)
            });
            ret.Add(new SelectListItem()
            {
                Text = "女",
                Value = "2",
                Selected = (selectedValue == 2)
            });
            ret.Add(new SelectListItem()
            {
                Text = "其他",
                Value = "3",
                Selected = (selectedValue == 3)
            });

            return ret;
        }
        #endregion 

        #region 初始化选项列表-职位
        /// <summary>
        /// 初始化选项列表-职位
        /// </summary>
        private static IList<SelectListItem> InitSelectListForDuty(int selectedValue)
        {
            IList<SelectListItem> ret = new List<SelectListItem>();
            ret.Add(new SelectListItem()
            {
                Text = "请选择",
                Value = "0",
                Selected = (selectedValue == 0)
            });
            IList<EmployeeDuty> allDuty = Container.Instance.Resolve<EmployeeDutyService>().GetAll();
            foreach (var item in allDuty)
            {
                ret.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString(),
                    Selected = (selectedValue == item.ID)
                });
            }

            return ret;
        }
        #endregion 

    }
}