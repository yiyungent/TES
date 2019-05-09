using Castle.ActiveRecord;
using Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 部门
    /// </summary>
    [ActiveRecord]
    public class Department : BaseEntity<Department>
    {
        /// <summary>
        /// 部门名
        /// </summary>
        [Display(Name = "部门名")]
        [Property(Length = 30, NotNull = true)]
        public string Name { get; set; }

        [Display(Name = "父级部门")]
        [BelongsTo(Column = "ParentId")]
        public Department ParentDept { get; set; }

        [Display(Name = "子部门列表")]
        [HasMany(ColumnKey = "ParentId")]
        public IList<Department> Children { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Display(Name = "排序码")]
        [Property(NotNull = true)]
        public int SortCode { get; set; }

        /// <summary>
        /// 部门类型
        /// </summary>
        [Display(Name = "部门类型")]
        [Property(NotNull = false)]
        public int DeptType { get; set; }
    }
}
