using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Framework.HtmlHelpers
{
    public static class PageHelper
    {
        public static MvcHtmlString PageLinks(this HtmlHelper value, PageInfo pageInfo, Func<int, string> pageUrl)
        {
            StringBuilder sbResult = new StringBuilder();

            #region 上一页
            if (pageInfo.PageIndex > 1)
            {
                TagBuilder prevTag = new TagBuilder("a");
                prevTag.MergeAttribute("href", pageUrl(pageInfo.PageIndex - 1));
                prevTag.InnerHtml = "&laquo;";
                prevTag.AddCssClass("btn btn-default");
                sbResult.Append(prevTag.ToString());
            }
            #endregion

            #region 首页按钮
            TagBuilder homeTag = new TagBuilder("a");
            homeTag.MergeAttribute("href", pageUrl(1));
            homeTag.InnerHtml = "首页";
            homeTag.AddCssClass("btn btn-default");
            sbResult.Append(homeTag.ToString());
            #endregion
            #region 首页后省略号
            if (pageInfo.PageIndex >= pageInfo.MaxLinkCount)
            {
                TagBuilder tag = new TagBuilder("span");
                tag.InnerHtml = "…";
                tag.AddCssClass("btn btn-default disabled");
                tag.MergeAttribute("disabled", "disabled");
                sbResult.Append(tag.ToString());
            }
            #endregion

            #region 普通页面按钮--页码条

            // 显示分页工具条中普通页码
            // 显示第一个页码
            int begin;
            // 显示最后一个页码
            int end;
            // 第一页码
            begin = pageInfo.PageIndex - (pageInfo.MaxLinkCount / 2);
            if (begin < 1)
            {
                // 第一个页码 不能小于1
                begin = 1;
            }

            // 最大页码
            end = begin + (pageInfo.MaxLinkCount - 1);
            if (end > pageInfo.TotalPages)
            {
                // 最后一个页码不能大于总页数
                end = pageInfo.TotalPages;
                // 修正begin 的值, 若页码条容量为10, 理论上 begin 是 end - 9
                begin = end - (pageInfo.MaxLinkCount - 1);
                if (begin < 1)
                {
                    // 第一个页码 不能小于1
                    begin = 1;
                }
            }
            for (int i = begin; i <= end; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
                if (i == pageInfo.PageIndex)
                {
                    tag.AddCssClass("btn btn-primary selected");
                }
                else
                {
                    tag.AddCssClass("btn btn-default");
                }
                sbResult.Append(tag.ToString());
            }
            #endregion

            #region 尾页前省略号
            if (pageInfo.PageIndex <= pageInfo.TotalPages - pageInfo.MaxLinkCount + 1)
            {
                TagBuilder tag = new TagBuilder("span");
                tag.InnerHtml = "…";
                tag.AddCssClass("btn btn-default disabled");
                tag.MergeAttribute("disabled", "disabled");
                sbResult.Append(tag.ToString());
            }
            #endregion
            #region 尾页按钮
            TagBuilder endTag = new TagBuilder("a");
            endTag.MergeAttribute("href", pageUrl(pageInfo.TotalPages));
            endTag.InnerHtml = "尾页";
            endTag.AddCssClass("btn btn-default");
            sbResult.Append(endTag.ToString());
            #endregion

            #region 下一页
            if (pageInfo.PageIndex < pageInfo.TotalPages)
            {
                TagBuilder nextTag = new TagBuilder("a");
                nextTag.MergeAttribute("href", pageUrl(pageInfo.PageIndex + 1));
                nextTag.InnerHtml = "&raquo;";
                nextTag.AddCssClass("btn btn-default");
                sbResult.Append(nextTag.ToString());
            }
            #endregion

            return MvcHtmlString.Create(sbResult.ToString());
        }
    }
}