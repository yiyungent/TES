using Castle.ActiveRecord;
using Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    /// <summary>
    /// 班级表
    /// </summary>
    [ActiveRecord]
    public class ClazzInfo : BaseEntity<ClazzInfo>
    {
        /// <summary>
        /// 班号
        /// </summary>
        [Display(Name = "班号")]
        [Property(Length = 30, NotNull = false)]
        public string ClazzCode { get; set; }

        #region Relationship

        /// <summary>
        /// 学生列表
        ///     一对多关系
        /// </summary>
        [Display(Name = "学生列表")]
        [HasMany(ColumnKey = "ClazzId")]
        public IList<StudentInfo> StudentList { get; set; }

        [Display(Name = "课程表列表")]
        [HasMany(ColumnKey = "ClazzId")]
        public IList<CourseTable> CourseTableList { get; set; }

        #endregion

        #region Helper

        /// <summary>
        /// 改班学生人数
        /// </summary>
        public int StudentCount
        {
            get
            {
                return StudentList.Count;
            }
        }

        #endregion
    }
}
