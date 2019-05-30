using Castle.ActiveRecord;
using Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Domain
{
    /// <summary>
    /// 实体类：评价指标
    /// </summary>
    [ActiveRecord]
    public class NormTarget : BaseEntity<NormTarget>
    {
        ///<summary>
        /// 指标名称
        /// </summary>
        [Display(Name = "指标名称")]
        [Property(Length = 30, NotNull = true)]
        public string Name { get; set; }

        ///<summary>
        /// 指标类型
        /// </summary>
        [Display(Name = "指标类型")]
        [BelongsTo(Column = "ParentId")]
        public NormType NormType { get; set; }

        ///<summary>
        /// 权重
        /// </summary>
        [Display(Name = "权重")]
        [Property(NotNull = true)]
        public decimal Weight { get; set; }

        ///<summary>
        /// 排序码
        /// </summary>
        [Display(Name = "排序码")]
        [Property(NotNull = true)]
        public int SortCode { get; set; }

        ///<summary>
        /// 上级指标
        /// </summary>
        [Display(Name = "上级指标")]
        [BelongsTo(Column = "ParentId")]
        public NormTarget ParentTarget { get; set; }

        ///<summary>
        /// 下级指标
        /// </summary>
        [Display(Name = "下级指标")]
        [HasMany(ColumnKey = "ParentId")]
        public IList<NormTarget> ChildTargetList { get; set; }

        ///<summary>
        /// 指标选项
        /// </summary>
        [Display(Name = "指标选项")]
        [HasMany(ColumnKey = "NormTargetId")]
        public IList<Options> OptionsList { get; set; }
    }
}
