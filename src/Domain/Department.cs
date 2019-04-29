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
    /// 系部(部门)
    /// </summary>
    [ActiveRecord]
    public class Department : BaseEntity<Department>
    {
        /// <summary>
        /// 该系有哪些班级
        /// </summary>
        [Display(Name = "班级列表")]
        [HasMany(ColumnKey = "DepartmentId")]
        public IList<ClazzInfo> ClazzInfoList { get; set; }

    }
}
