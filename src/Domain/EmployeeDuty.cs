using Castle.ActiveRecord;
using Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    [ActiveRecord]
    public class EmployeeDuty : BaseEntity<EmployeeDuty>
    {
        /// <summary>
        /// 职位名称
        /// </summary>
        [Display(Name = "职位名称")]
        [Property(NotNull = true, Length = 30)]
        public string Name { get; set; }

        #region 废弃
        ///// <summary>
        ///// 要评价的哪些职位的员工
        /////     存放职位的ID列表字符串: 1,5,7,3,6,1
        ///// </summary>
        //[Property(NotNull = false, Length = 100)]
        //public string EvaDutyIds { get; set; } 
        #endregion

        /// <summary>
        /// 要评价哪些职位
        /// </summary>
        [Display(Name = "要评价的职位列表")]
        [HasAndBelongsToMany(Table = "EvaDuty_EvaedDuty", ColumnKey = "EvaDutyListId", ColumnRef = "EvaedDutyListId")]
        public IList<EmployeeDuty> EvaDutyList { get; set; }

        /// <summary>
        /// 可以被哪些职位评价
        /// </summary>
        [HasAndBelongsToMany(Table = "EvaDuty_EvaedDuty", ColumnKey = "EvaedDutyListId", ColumnRef = "EvaDutyListId")]
        public IList<EmployeeDuty> EvaedDutyList { get; set; }

        /// <summary>
        /// 使用的评价类型
        /// </summary>
        [Display(Name = "评价类型 ")]
        [BelongsTo(Column = "NormTypeId")]
        public NormType NormType { get; set; }
    }
}
