using Castle.ActiveRecord;
using Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    [ActiveRecord]
    public class CourseInfo : BaseEntity<CourseInfo>
    {
        /// <summary>
        /// 课程名
        /// </summary>
        [Display(Name = "课程名")]
        [Property(Length = 30, NotNull = true)]
        public string Name { get; set; }

        /// <summary>
        /// 课程代号
        /// </summary>
        [Display(Name = "课程代号")]
        [Property(Length = 30, NotNull = true)]
        public string CourseCode { get; set; }

        #region Relationship

        [Display(Name = "课程表列表")]
        [HasMany(ColumnKey = "CourseId", Cascade = ManyRelationCascadeEnum.All)]
        public IList<CourseTable> CourseTableList { get; set; }

        #endregion
    }
}
