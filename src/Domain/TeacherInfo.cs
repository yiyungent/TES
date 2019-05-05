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
        /// 工号（注册时，此作为 绑定用户的 LoginAccount）
        /// </summary>
        [Display(Name = "工号")]
        [Property(Length = 30, NotNull = true)]
        public string TeacherCode { get; set; }

        #region Relationship

        /// <summary>
        /// 绑定用户账号
        /// <para>在创建教师时，同时创建其绑定用户</para>
        /// </summary>
        [Display(Name = "绑定用户账号")]
        [Property(Length = 30, NotNull = true)]
        public string UserInfo_Account { get; set; }

        /// <summary>
        /// 教师所教班级
        /// </summary>
        [HasAndBelongsToMany(Table = "Teacher_ClazzInfo", ColumnKey = "TeacherId", ColumnRef = "ClazzId")]
        public IList<ClazzInfo> ClazzInfoList { get; set; }

        #endregion
    }
}
