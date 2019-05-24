using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Domain;

namespace WebUI.Areas.Admin.Models.NormTypeM
{
    public class NormTypeForEditViewModel
    {
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

        public static explicit operator NormTypeForEditViewModel(NormType dbModel)
        {
            NormTypeForEditViewModel rtn = null;

            rtn = new NormTypeForEditViewModel
            {
                ID = dbModel.ID,
                InputColor = dbModel.Color,
                InputName = dbModel.Name,
                InputNormTypeCode = dbModel.NormTypeCode,
                InputSortCode = dbModel.SortCode,
                InputWeight = dbModel.Weight
            };

            return rtn;
        }
    }
}