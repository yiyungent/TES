using Core;
using Domain;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Extensions;

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
            this.SelectListForDept = InitSelectListForDept(new Department(), 0);
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
                SelectListForDept = InitSelectListForDept(dbModel.Department, dbModel.Department?.ID ?? 0),
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
        private static IList<SelectListItem> InitSelectListForDept(Department self, int parentId)
        {
            IList<SelectListItem> ret = new List<SelectListItem>();
            ret.Add(new SelectListItem()
            {
                Text = "请选择",
                Value = "0",
                Selected = (parentId == 0)
            });
            #region 临时解决没有当前选中项问题
            ret.Add(new SelectListItem()
            {
                Text = self.DescDept(),
                Value = self.ID.ToString(),
                Selected = (parentId == self.ID)
            }); 
            #endregion

            IList<Department> all = Container.Instance.Resolve<DepartmentService>().GetAll();
            // 找出自己及后代的ID
            IList<int> idRange = new List<int>();
            GetIdRange(self, idRange, all);

            // not in 查询
            var find = from m in all
                       where idRange.Contains(m.ID) == false
                       select m;
            // 方案二：考虑层级的实现
            // 1.一级菜单
            var first = from m in find
                        where m.ParentDept == null
                        orderby m.SortCode
                        select m;
            foreach (var item in first)
            {
                AddSelfAndChildrenForDDL("", item, ret, find.ToList(), parentId);
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

        #region 辅助初始化选项列表-部门
        private static void AddSelfAndChildrenForDDL(string pref, Department self, IList<SelectListItem> target, IList<Department> all, int parentId)
        {
            // 1.添加自己
            target.Add(new SelectListItem()
            {
                Text = pref + self.Name,
                Value = self.ID.ToString(),
                Selected = (self.ID == parentId)
            });
            // 2.递归添加子女
            var child = from m in all
                        where m.ParentDept != null && m.ParentDept.ID == self.ID
                        orderby m.SortCode
                        select m;
            foreach (var item in child)
            {
                AddSelfAndChildrenForDDL(pref + "--", item, target, all, parentId);
            }
        }


        /// <summary>
        /// 递归方法：添加自己及其子女
        /// </summary>
        /// <param name="self"></param>
        /// <param name="idRange"></param>
        /// <param name="all"></param>
        private static void GetIdRange(Department self, IList<int> idRange, IList<Department> all)
        {
            // 添加自己
            idRange.Add(self.ID);

            // 关于子女循环
            // 第二种解决方法：
            if (self.Children == null) return;
            foreach (var item in self.Children)
            {
                // 递归调用---添加自己和自己的子女
                GetIdRange(item, idRange, all);
            }
        }
        #endregion
    }
}