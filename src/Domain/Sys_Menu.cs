using Castle.ActiveRecord;
using Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    /// <summary>
    /// 实体类: 系统菜单--仅用于后台显示菜单列表
    /// </summary>
    [ActiveRecord]
    public partial class Sys_Menu : BaseEntity<Sys_Menu>
    {

    }
}
