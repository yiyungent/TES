using Castle.ActiveRecord;
using Domain.Base;
using System.ComponentModel.DataAnnotations;
namespace Domain
{
    /// <summary>
    /// 实体类：评价类型
    /// </summary>
    [ActiveRecord]
    public class NormType : BaseEntity<NormType>
    {
        ///<summary>
        /// 评价类型名称
        /// </summary>
        [Display(Name = "评价类型名称")]
        [Property(Length = 30, NotNull = true)]
        public string Name { get; set; }

        ///<summary>
        /// 排序码
        /// </summary>
        [Display(Name = "排序码")]
        [Property(NotNull = true)]
        public int SortCode { get; set; }

        ///<summary>
        /// 权重
        /// </summary>
        [Display(Name = "权重")]
        [Property(NotNull = true)]
        public decimal Weight { get; set; }

        ///<summary>
        ///颜色值
        /// </summary>
        [Display(Name = "颜色值")]
        [Property(Length = 30)]
        public string Color { get; set; }

        ///<summary>
        ///评价类型代码
        /// </summary>
        [Display(Name = "评价类型代码")]
        [Property(Length = 30, NotNull = true)]
        public string NormTypeCode { get; set; }
    }
}
