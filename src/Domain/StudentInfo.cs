using Castle.ActiveRecord;
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
        /// 学号，作为登录账号
        /// </summary>
        [Display(Name = "学号")]
        [Property(Length = 30, NotNull = true)]
        public string StudentCode { get; set; }


        /// <summary>
        /// 若为学生，则为所在班级
        /// </summary>
        [BelongsTo(Column = "ClazzId")]
        public ClazzInfo ClazzInfo { get; set; }

        /// <summary>
        /// 用户头像Url地址
        /// </summary>
        [Display(Name = "头像")]
        [Property(Length = 50, NotNull = false)]
        public string Avatar { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Display(Name = "邮箱")]
        [Property(Length = 50, NotNull = false)]
        public string Email { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        [Display(Name = "注册时间")]
        [Property(NotNull = true)]
        public DateTime RegTime { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码")]
        [Property(Length = 64, NotNull = true)]
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }

        /// <summary>
        /// 口令
        ///     仅用作 “记住我” 记住 登录状态时 使用
        /// </summary>
        [Display(Name = "口令")]
        [Property(Length = 40, NotNull = false)]
        public string Token { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [Property(NotNull = true)]
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 状态
        ///     0: 正常
        ///     1: 禁用
        /// </summary>
        [Display(Name = "状态")]
        [Property]
        public int Status { get; set; }

        /// <summary>
        /// 担任角色列表
        ///     多对多关系
        /// </summary>
        [Display(Name = "担任角色列表")]
        [HasAndBelongsToMany(Table = "Role_User", ColumnKey = "UserId", ColumnRef = "RoleId")]
        public IList<RoleInfo> RoleInfoList { get; set; }
    }
}
