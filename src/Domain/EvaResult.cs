using Castle.ActiveRecord;
using Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
namespace Domain
{
    /// <summary>
    /// 实体类：评价结果
    /// </summary>
    [ActiveRecord]
    public class EvaResult : BaseEntity<EvaResult>
    {
        ///<summary>
        /// 被评价人
        /// </summary>
        [Display(Name = "被评价人")]
        [BelongsTo(Column = "TeacherId")]
        public EmployeeInfo Teacher { get; set; }

        ///<summary>
        /// 评价类型
        /// </summary>
        [Display(Name = "评价类型")]
        [BelongsTo(Column = "NormTypeId")]
        public NormType NormType { get; set; }

        ///<summary>
        /// 评价任务
        /// </summary>
        [Display(Name = "评价任务")]
        [BelongsTo(Column = "EvaluateTaskId")]
        public EvaTask EvaluateTask { get; set; }

        ///<summary>
        ///计算时间
        /// </summary>
        [Display(Name = "计算时间")]
        [Property(NotNull = true)]
        public DateTime CaculateTime { get; set; }

        ///<summary>
        /// 成绩
        /// </summary>
        [Display(Name = "成绩")]
        [Property(NotNull = true)]
        public decimal Score { get; set; }
    }
}
