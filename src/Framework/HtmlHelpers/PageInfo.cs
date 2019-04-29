using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Framework.HtmlHelpers
{
    public class PageInfo
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalRecordCount { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        /// <summary>
        /// 最大普通页码按钮数目-----页码条容量
        ///     首页，尾页按钮不算普通页码按钮
        /// </summary>
        public int MaxLinkCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((double)TotalRecordCount / PageSize);
            }
        }
    }
}