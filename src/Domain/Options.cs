
using Castle.ActiveRecord;
using Domain.Base;
using System.ComponentModel.DataAnnotations;
namespace Domain
{
    /// <summary>
    /// 实体类：指标选项
    /// </summary>
    [ActiveRecord]
    public class Options : BaseEntity<Options>
    {
        ///<summary>
        /// 选项内容
        /// </summary>
        [Display(Name = "选项内容")]
        [Property(Length = 200, NotNull = true)]
        public string Content { get; set; }

        ///<summary>
        /// 排序码
        /// </summary>
        [Display(Name = "排序码")]
        [Property(NotNull = true)]
        public int SortCode { get; set; }

        ///<summary>
        /// 分值
        /// </summary>
        [Display(Name = "分值")]
        [Property(NotNull = true)]
        public decimal Score { get; set; }

        ///<summary>
        /// 所属指标
        /// </summary>
        [Display(Name = "所属指标")]
        [BelongsTo(Column = "NormTargetId")]
        public NormTarget NormTarget { get; set; }
    }
}
