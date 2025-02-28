﻿using Castle.ActiveRecord;
using Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    /// <summary>
    /// 学生表
    /// </summary>
    [ActiveRecord]
    public class StudentInfo : BaseEntity<StudentInfo>
    {
        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        [Property(Length = 30, NotNull = true)]
        public string Name { get; set; }

        /// <summary>
        /// 学号，可作为登录账号
        /// </summary>
        [Display(Name = "学号")]
        [Property(Length = 30, NotNull = true)]
        public string StudentCode { get; set; }

        /// <summary>
        /// 绑定用户-用户ID
        /// </summary>
        [Property]
        public int? UID { get; set; }

        #region Relationship

        /// <summary>
        /// 若为学生，则为所在班级
        /// </summary>
        [BelongsTo(Column = "ClazzId")]
        public ClazzInfo ClazzInfo { get; set; }

        #endregion
    }
}
