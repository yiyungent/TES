using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Base
{
    public class CommonEntity
    {
    }

    /// <summary>
    /// 操作地址
    /// </summary>
    public class AreaCAItem
    {
        public string AreaName { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }
    }
}
