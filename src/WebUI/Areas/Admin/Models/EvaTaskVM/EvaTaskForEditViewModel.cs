using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;
using Domain;
using Service;

namespace WebUI.Areas.Admin.Models.EvaTaskVM
{
    public class EvaTaskForEditViewModel
    {
        #region Properties

        [Display(Name = "编号")]
        public int ID { get; set; }

        ///<summary>
        /// 任务名称
        /// </summary>
        [Display(Name = "任务名称")]
        [Required]
        public string InputName { get; set; }

        /// <summary>
        /// 任务号
        /// </summary>
        [Display(Name = "任务号")]
        [Required]
        public string InputEvaTaskCode { get; set; }

        ///<summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        [Required]
        public DateTime InputStartDate { get; set; }

        ///<summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        [Required]
        public DateTime InputEndDate { get; set; }

        #endregion

        #region Ctor
        public EvaTaskForEditViewModel()
        {
           
        }
        #endregion

        #region Methods

        #region 数据库模型->视图模型
        public static explicit operator EvaTaskForEditViewModel(EvaTask dbModel)
        {
            EvaTaskForEditViewModel viewModel = new EvaTaskForEditViewModel
            {
                ID = dbModel.ID,
                InputName = dbModel.Name,
                InputEvaTaskCode = dbModel.EvaTaskCode,
                InputStartDate = dbModel.StartDate,
                InputEndDate = dbModel.EndDate,
            };

            return viewModel;
        }
        #endregion

        #region 输入模型->数据库模型
        public static explicit operator EvaTask(EvaTaskForEditViewModel inputModel)
        {
            EvaTask dbModel = null;
            if (inputModel.ID == 0)
            {
                // 创建
                dbModel = new EvaTask();
            }
            else
            {
                // 修改
                dbModel = Container.Instance.Resolve<EvaTaskService>().GetEntity(inputModel.ID);
            }
            dbModel.Name = inputModel.InputName?.Trim();
            dbModel.StartDate = inputModel.InputStartDate;
            dbModel.EndDate = inputModel.InputEndDate;
            dbModel.EvaTaskCode = inputModel.InputEvaTaskCode?.Trim();

            return dbModel;
        }
        #endregion

        #endregion
    }
}