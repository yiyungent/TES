using Core;
using Domain;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Areas.Admin.Models.EmployeeDutyVM
{
    public class EmployeeDutyForEditViewModel
    {
        #region Properties

        [Display(Name = "编号")]
        public int ID { get; set; }

        ///<summary>
        /// 任务名称
        /// </summary>
        [Display(Name = "职位名称")]
        [Required]
        public string InputName { get; set; }

        ///<summary>
        /// 仅作展示，选择评价类型
        /// </summary>
        [Display(Name = "评价类型")]
        public IList<SelectListItem> SelectListForNormType { get; set; }

        /// <summary>
        /// 仅作接收, 被选中的评价类型
        /// </summary>
        public int SelectedValForNormType { get; set; }

        ///<summary>
        /// 仅作展示，选择评价职位
        /// </summary>
        [Display(Name = "评价职位")]
        public IList<SelectListItem> SelectListForEvaDuty { get; set; }

        /// <summary>
        /// 仅作接收, 被选中的评价职位
        /// </summary>
        public int[] SelectedValArrForEvaDuty { get; set; }

        #endregion

        #region Ctor
        public EmployeeDutyForEditViewModel()
        {
            this.SelectListForNormType = InitSelectListForNormType(0);
            this.SelectedValForNormType = 0;
            this.SelectListForEvaDuty = InitSelectListForEvaDuty();
        }
        #endregion

        #region Methods

        #region 数据库模型->视图模型
        public static explicit operator EmployeeDutyForEditViewModel(EmployeeDuty dbModel)
        {
            EmployeeDutyForEditViewModel viewModel = new EmployeeDutyForEditViewModel
            {
                ID = dbModel.ID,
                InputName = dbModel.Name,
                SelectListForNormType = InitSelectListForNormType(dbModel.NormType?.ID ?? 0),
                SelectedValForNormType = dbModel.NormType?.ID ?? 0,
                SelectListForEvaDuty = InitSelectListForEvaDuty(dbModel.EvaDutyList.Select(m => m.ID).ToArray()),
                SelectedValArrForEvaDuty = dbModel.EvaDutyList.Select(m => m.ID).ToArray()
            };

            return viewModel;
        }
        #endregion

        #region 输入模型->数据库模型
        public static explicit operator EmployeeDuty(EmployeeDutyForEditViewModel inputModel)
        {
            EmployeeDuty dbModel = null;
            if (inputModel.ID == 0)
            {
                // 创建
                dbModel = new EmployeeDuty();
            }
            else
            {
                // 修改
                dbModel = Container.Instance.Resolve<EmployeeDutyService>().GetEntity(inputModel.ID);
            }
            dbModel.Name = inputModel.InputName?.Trim();
            dbModel.NormType = new NormType { ID = inputModel.SelectedValForNormType };
            dbModel.EvaDutyList = new List<EmployeeDuty>();
            foreach (int item in inputModel.SelectedValArrForEvaDuty)
            {
                dbModel.EvaDutyList.Add(new EmployeeDuty { ID = item });
            }

            return dbModel;
        }


        #endregion

        #region 初始化选项列表-评价类型
        /// <summary>
        /// 初始化选项列表-评价类型
        /// </summary>
        private static IList<SelectListItem> InitSelectListForNormType(int selectedValue)
        {
            IList<SelectListItem> ret = new List<SelectListItem>();
            ret.Add(new SelectListItem()
            {
                Text = "请选择",
                Value = "0",
                Selected = (selectedValue == 0)
            });
            IList<NormType> allNormType = Container.Instance.Resolve<NormTypeService>().GetAll();
            foreach (var item in allNormType)
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

        #region 初始化选项列表-评价职位
        /// <summary>
        /// 初始化选项列表-评价职位
        /// </summary>
        private static IList<SelectListItem> InitSelectListForEvaDuty(params int[] selectedValueArr)
        {
            IList<SelectListItem> ret = new List<SelectListItem>();
            IList<EmployeeDuty> allDuty = Container.Instance.Resolve<EmployeeDutyService>().GetAll();
            foreach (var item in allDuty)
            {
                ret.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID.ToString(),
                    Selected = selectedValueArr.Contains(item.ID)
                });
            }

            return ret;
        }
        #endregion

        #endregion
    }
}