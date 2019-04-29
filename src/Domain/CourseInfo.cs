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

        /// <summary>
        /// 有此课程的班级列表
        /// </summary>
        [Display(Name = "有此课程的班级列表")]
        [HasAndBelongsToMany(Table = "Course_ClazzInfo", ColumnKey = "CourseId", ColumnRef = "ClazzId")]
        public IList<ClazzInfo> ClazzInfoList { get; set; }
    }
}
