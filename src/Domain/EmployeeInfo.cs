﻿using Castle.ActiveRecord;
using Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    [ActiveRecord]
    public class EmployeeInfo : BaseEntity<EmployeeInfo>
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        [Property(Length = 30, NotNull = true)]
        public string Name { get; set; }

        /// <summary>
        /// 工号（注册时，此作为 绑定用户的 LoginAccount）
        /// </summary>
        [Display(Name = "工号")]
        [Property(Length = 30, NotNull = true)]
        public string EmployeeCode { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        [Display(Name = "入职时间")]
        [Property(NotNull = true)]
        public DateTime Start_Time { get; set; }

        /// <summary>
        /// 离职时间
        /// </summary>
        [Display(Name = "离职时间")]
        [Property(NotNull = false)]
        public DateTime End_Time { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        [Property(NotNull = false)]
        public int Sex { get; set; }

        #region Relationship

        /// <summary>
        /// 绑定用户账号
        /// <para>在创建员工时，同时创建其绑定用户</para>
        /// </summary>
        [Display(Name = "绑定用户账号")]
        [Property(Length = 30, NotNull = true)]
        public string UserInfo_Account { get; set; }

        [Display(Name = "所在部门")]
        [BelongsTo(Column = "DeptId")]
        public Department Department { get; set; }

        [Display(Name = "课程表列表")]
        [HasMany(ColumnKey = "CourseId")]
        public IList<CourseTable> CourseTableList { get; set; }

        #endregion
    }
}
