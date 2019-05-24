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

        ///<summary>
        /// 仅作展示，选择状态
        /// </summary>
        [Display(Name = "状态")]
        public IList<SelectListItem> SelectListForStatus { get; set; }

        /// <summary>
        /// 仅作接收, 被选中的状态
        /// </summary>
        public int SelectedValForStatus { get; set; }

        #endregion

        #region Ctor
        public EvaTaskForEditViewModel()
        {
            this.SelectListForStatus = InitSelectListForStatus(0);
            this.SelectedValForStatus = 0;
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
                SelectListForStatus = InitSelectListForStatus(dbModel.Status),

                // DropDownList BUG 记录
                // https://www.2cto.com/kf/201110/109212.html
                // https://blog.csdn.net/distantmoon/article/details/42393905?locationNum=9
                // @Html.DropDownListFor(m => m.SelectedStatusVal, Model.SelectListForStatus)
                // 无论是否有For的版本，都注意第一个生成id,name值的参数，
                // 如果m模型中有相同名属性，则会与它相关联，可能导致无法仅根据 第二个参数 生成正确的选中项
                // 解决BUG， 为此属性同样赋值上应该选中的项的值
                SelectedValForStatus = dbModel.Status
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
            dbModel.Status = inputModel.SelectedValForStatus;

            return dbModel;
        }
        #endregion

        #region 初始化选项列表-状态
        /// <summary>
        /// 初始化选项列表-状态
        /// </summary>
        private static IList<SelectListItem> InitSelectListForStatus(int selectedValue)
        {
            IList<SelectListItem> ret = new List<SelectListItem>();
            ret.Add(new SelectListItem()
            {
                Text = "请选择",
                Value = "0",
                Selected = (selectedValue == 0)
            });
            Dictionary<int, string> keyValuePairs = new Dictionary<int, string>
            {
                { 1, "待开启" },
                { 2, "正在评价" },
                { 3, "评价结束" }
            };
            foreach (var item in keyValuePairs)
            {
                ret.Add(new SelectListItem()
                {
                    Text = item.Value,
                    Value = item.Key.ToString(),
                    Selected = (selectedValue == item.Key)
                });
            }

            return ret;
        }
        #endregion 

        #endregion
    }
}