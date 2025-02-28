﻿using Castle.ActiveRecord;
using Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
namespace Domain
{
    /// <summary>
    /// 实体类：评价任务
    /// </summary>
    [ActiveRecord]
    public class EvaTask : BaseEntity<EvaTask>
    {
        ///<summary>
        /// 任务名称
        /// </summary>
        [Display(Name = "任务名称")]
        [Property(Length = 30, NotNull = true)]
        public string Name { get; set; }

        /// <summary>
        /// 任务号
        /// </summary>
        [Display(Name = "任务号")]
        [Property(Length = 30, NotNull = true)]
        public string EvaTaskCode { get; set; }

        ///<summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        [Property(NotNull = true)]
        public DateTime StartDate { get; set; }

        ///<summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        [Property(NotNull = true)]
        public DateTime EndDate { get; set; }

        #region 废弃
        /////<summary>
        ///// 状态
        /////      1：待开启
        /////      2：正在评价
        /////      3：评价结束
        ///// </summary>
        //[Display(Name = "状态")]
        //[Property(NotNull = true)]
        //public int Status { get; set; }
        #endregion

        ///<summary>
        /// 状态
        ///      1：待开启
        ///      2：正在评价
        ///      3：评价结束
        /// </summary>
        [Display(Name = "状态")]
        public int Status
        {
            get
            {
                DateTime nowTime = DateTime.Now;
                if (nowTime < StartDate)
                {
                    return 1;
                }
                else if (nowTime < EndDate)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            set
            {

            }
        }

    }
}
