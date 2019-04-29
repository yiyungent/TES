﻿using Castle.ActiveRecord;
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

        /// <summary>
        /// 该班课程列表
        /// </summary>
        [Display(Name = "课程列表")]
        [HasAndBelongsToMany(Table = "Course_ClazzInfo", ColumnKey = "ClazzId", ColumnRef = "CourseId")]
        public IList<CourseInfo> CourseInfoList { get; set; }

        /// <summary>
        /// 所属系
        /// </summary>
        [Display(Name = "所属系")]
        [BelongsTo(Column = "DepartmentId")]
        public Department Department { get; set; }

        /// <summary>
        /// 班级拥有授课老师
        /// </summary>
        [HasAndBelongsToMany(Table = "Teacher_ClazzInfo", ColumnKey = "ClazzId", ColumnRef = "TeacherId")]
        public IList<TeacherInfo> TeacherInfoList { get; set; }

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
