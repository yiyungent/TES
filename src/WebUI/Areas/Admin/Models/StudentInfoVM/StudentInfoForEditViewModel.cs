using Core;
using Domain;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Areas.Admin.Models.StudentInfoVM
{
    public class StudentInfoForEditViewModel
    {
        [Display(Name = "编号")]
        public int ID { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "姓名")]
        [Required]
        public string InputName { get; set; }

        /// <summary>
        /// 学号，可作为登录账号
        /// </summary>
        [Display(Name = "学号")]
        [Required]
        public string InputStudentCode { get; set; }

        ///<summary>
        /// 仅作展示，选择班级
        /// </summary>
        [Display(Name = "班级")]
        public IList<SelectListItem> SelectListForClazz { get; set; }

        /// <summary>
        /// 仅作接收, 被选中的班级
        /// </summary>
        public int SelectedValForClazz { get; set; }

        #region Ctor
        public StudentInfoForEditViewModel()
        {
            this.SelectListForClazz = InitSelectListForClazz(0);
            this.SelectedValForClazz = 0;
        }
        #endregion

        #region 数据库模型->视图模型
        public static explicit operator StudentInfoForEditViewModel(StudentInfo dbModel)
        {
            StudentInfoForEditViewModel viewModel = new StudentInfoForEditViewModel
            {
                ID = dbModel.ID,
                InputName = dbModel.Name,
                InputStudentCode = dbModel.StudentCode,
                SelectListForClazz = InitSelectListForClazz(dbModel.ClazzInfo?.ID ?? 0),
                SelectedValForClazz = dbModel.ClazzInfo?.ID ?? 0
            };

            return viewModel;
        }
        #endregion

        #region 输入模型->数据库模型
        public static explicit operator StudentInfo(StudentInfoForEditViewModel inputModel)
        {
            StudentInfo dbModel = null;
            if (inputModel.ID == 0)
            {
                // 创建
                dbModel = new StudentInfo();
            }
            else
            {
                // 修改
                dbModel = Container.Instance.Resolve<StudentInfoService>().GetEntity(inputModel.ID);
            }
            dbModel.Name = inputModel.InputName?.Trim();
            dbModel.StudentCode = inputModel.InputStudentCode;
            dbModel.ClazzInfo = new ClazzInfo { ID = inputModel.SelectedValForClazz };

            return dbModel;
        }
        #endregion

        #region 初始化选项列表-班级
        /// <summary>
        /// 初始化选项列表-班级
        /// </summary>
        private static IList<SelectListItem> InitSelectListForClazz(int selectedValue)
        {
            IList<SelectListItem> ret = new List<SelectListItem>();
            ret.Add(new SelectListItem()
            {
                Text = "请选择",
                Value = "0",
                Selected = (selectedValue == 0)
            });
            IList<ClazzInfo> list = Container.Instance.Resolve<ClazzInfoService>().GetAll();
            foreach (var item in list)
            {
                ret.Add(new SelectListItem()
                {
                    Text = item.ClazzCode,
                    Value = item.ID.ToString(),
                    Selected = (selectedValue == item.ID)
                });
            }

            return ret;
        }
        #endregion 

    }
}