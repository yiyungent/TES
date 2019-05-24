using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Core;
using Domain;
using Service;

namespace WebUI.Areas.Admin.Models.NormTypeVM
{
    public class NormTypeForEditViewModel
    {
        #region Properties

        /// <summary>
        /// ID
        /// </summary>
        [Display(Name = "编号")]
        public int ID { get; set; }

        ///<summary>
        /// 评价类型名称
        /// </summary>
        [Display(Name = "评价类型名称")]
        [Required]
        public string InputName { get; set; }

        ///<summary>
        /// 排序码
        /// </summary>
        [Display(Name = "排序码")]
        [Required]
        public int InputSortCode { get; set; }

        ///<summary>
        /// 权重
        /// </summary>
        [Display(Name = "权重")]
        [Required]
        public decimal InputWeight { get; set; }

        ///<summary>
        ///颜色值
        /// </summary>
        [Display(Name = "颜色值")]
        public string InputColor { get; set; }

        ///<summary>
        ///评价类型代码
        /// </summary>
        [Display(Name = "评价类型代码")]
        [Required]
        public string InputNormTypeCode { get; set; }

        #endregion

        #region Methods

        #region 数据库模型->输入\视图模型
        public static explicit operator NormTypeForEditViewModel(NormType dbModel)
        {
            NormTypeForEditViewModel viewModel = null;

            viewModel = new NormTypeForEditViewModel
            {
                ID = dbModel.ID,
                InputColor = dbModel.Color,
                InputName = dbModel.Name,
                InputNormTypeCode = dbModel.NormTypeCode,
                InputSortCode = dbModel.SortCode,
                InputWeight = dbModel.Weight
            };

            return viewModel;
        }
        #endregion

        #region 输入模型->数据库模型
        public static explicit operator NormType(NormTypeForEditViewModel inputModel)
        {
            NormType dbModel = null;
            if (inputModel.ID == 0)
            {
                // 创建
                dbModel = new NormType();
            }
            else
            {
                // 修改
                dbModel = Container.Instance.Resolve<NormTypeService>().GetEntity(inputModel.ID);
            }
            dbModel.Name = inputModel.InputName?.Trim();
            dbModel.Color = inputModel.InputColor?.Trim();
            dbModel.SortCode = inputModel.InputSortCode;
            dbModel.Weight = inputModel.InputWeight;
            dbModel.NormTypeCode = inputModel.InputNormTypeCode;

            return dbModel;
        }
        #endregion 

        #endregion
    }
}