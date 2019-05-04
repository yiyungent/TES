using Castle.ActiveRecord;
using Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    [ActiveRecord]
    public class TeacherInfo : BaseEntity<TeacherInfo>
    {
        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名")]
        [Property(Length = 30, NotNull = true)]
        public string Name { get; set; }

        /// <summary>
        /// 教师工号，作为登录账号
        /// </summary>
        [Display(Name = "教师工号")]
        [Property(Length = 30, NotNull = true)]
        public string TeacherCode { get; set; }

        #region Relationship

        /// <summary>
        /// 教师所教班级
        /// </summary>
        [HasAndBelongsToMany(Table = "Teacher_ClazzInfo", ColumnKey = "TeacherId", ColumnRef = "ClazzId")]
        public IList<ClazzInfo> ClazzInfoList { get; set; }

        #endregion
    }
}
