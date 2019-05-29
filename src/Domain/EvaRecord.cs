using Castle.ActiveRecord;
using Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
namespace Domain
{
    /// <summary>
    /// 实体类：评价记录
    /// </summary>
    [ActiveRecord]
    public class EvaRecord : BaseEntity<EvaRecord>
    {
        /// <summary>
        /// 评价人
        /// </summary>
        [Display(Name = "评价人")]
        [BelongsTo(Column = "EvaorId")]
        public UserInfo Evaluator { get; set; }

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
        ///评价时间
        /// </summary>
        [Display(Name = "评价时间")]
        [Property(NotNull = true)]
        public DateTime EvaDate { get; set; }

        ///<summary>
        /// 评价指标
        /// </summary>
        [Display(Name = "评价指标")]
        [BelongsTo(Column = "NormTargetId")]
        public NormTarget NormTarget { get; set; }

        ///<summary>
        /// 选中的评价选项
        /// </summary>
        [Display(Name = "选中的评价选项")]
        [BelongsTo(Column = "OptionsId")]
        public Options Options { get; set; }

        ///<summary>
        /// 评价任务
        /// </summary>
        [Display(Name = "评价任务")]
        [BelongsTo(Column = "EvaluateTaskId")]
        public EvaTask EvaluateTask { get; set; }
    }
}
