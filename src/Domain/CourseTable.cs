﻿using Castle.ActiveRecord;
using Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    /// <summary>
    /// 课程表
    /// </summary>
    [ActiveRecord]
    public class CourseTable : BaseEntity<CourseTable>
    {
        #region Relationship

        [Display(Name = "教师")]
        [BelongsTo(Column = "TeacherId")]
        public EmployeeInfo Teacher { get; set; }

        [Display(Name = "课程")]
        [BelongsTo(Column = "CourseId")]
        public CourseInfo Course { get; set; }

        [Display(Name = "班级")]
        [BelongsTo(Column = "ClazzId")]
        public ClazzInfo Clazz { get; set; }

        #endregion
    }
}
