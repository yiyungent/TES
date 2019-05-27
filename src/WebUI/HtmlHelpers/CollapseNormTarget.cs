using Core;
using Domain;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebUI.HtmlHelpers
{
    public static class CollapseNormTarget
    {
        private static IList<NormTarget> _allList;

        private const string _btnGroupFormat = "<div class='layui-btn-group' style='float: right;'><button class='layui-btn layui-btn-sm' onclick='onEdit({1})'>修改</button><button class='layui-btn layui-btn-sm' onclick='onSort({1})'>排序</button><button class='layui-btn layui-btn-sm' onclick='onOption({1})'>选项</button><button class='layui-btn layui-btn-sm layui-btn-danger' onclick='deleteOp({1})'>删除</button></div>";

        #region 产生树形列表
        public static MvcHtmlString GenerateCollapseNormTarget(this HtmlHelper value)
        {
            IList<NormTarget> allList = Container.Instance.Resolve<NormTargetService>().GetAll();
            _allList = allList;
            IList<NormTarget> firstDeptList = (from m in allList
                                               where m.ParentTarget == null
                                               orderby m.SortCode ascending
                                               select m).ToList();

            StringBuilder sbItemHtml = new StringBuilder();
            // 折叠菜单 layui-collapse
            sbItemHtml.Append("<div class=\"layui-collapse\" lay-accordion=\"\">");

            // 当前同等级别(一级菜单项) 的  菜单项(许多)
            foreach (var firstItem in firstDeptList)
            {
                // 折叠菜单项 layui-colla-item
                sbItemHtml.Append("<div class=\"layui-colla-item\">");
                // 此菜单项标题 layui-colla-title
                sbItemHtml.AppendFormat("<h2 class=\"layui-colla-title\">{0}" + _btnGroupFormat + "</h2>", firstItem.Name, firstItem.ID);

                // 此菜单项内容:  1. 无子项---<p></p>   2. 有子项---折叠菜单
                sbItemHtml.Append("<div class=\"layui-colla-content\">");
                // 里面又有许多项----此处开始进入 首次递归----------直到最后某项不再是任何项的父亲，则为 <p></p>
                // 深度优先--- 利用循环拿到每个菜单项,再进递归拿取其子菜单
                // 注意：如果当前菜单项已经无子项，则为 <p></p>，否则  继续向里递归寻找
                if (firstItem.ChildTargetList == null || firstItem.ChildTargetList.Count == 0)
                {
                    sbItemHtml.AppendFormat("<p>{0} 无子项</p>", firstItem.Name);
                }
                else
                {
                    // 否则又是一个折叠菜单
                    SubItemList(ref sbItemHtml, firstItem);
                }

                sbItemHtml.Append("</div>");

                sbItemHtml.Append("</div>");
            }

            sbItemHtml.Append("</div>");

            return MvcHtmlString.Create(sbItemHtml.ToString());
        }
        #endregion

        #region 递归查找子项
        private static void SubItemList(ref StringBuilder sbItemHtml, NormTarget currentItem)
        {
            IList<NormTarget> suDeptList = (from m in _allList
                                            where m.ParentTarget != null && m.ParentTarget.ID == currentItem.ID
                                            orderby m.SortCode ascending
                                            select m).ToList();
            // 此菜单项下又嵌套一个折叠菜单
            sbItemHtml.Append("<div class=\"layui-collapse\" lay-accordion=\"\">");
            foreach (var item in suDeptList)
            {
                sbItemHtml.Append("<div class=\"layui-colla-item\">");
                sbItemHtml.AppendFormat("<h2 class=\"layui-colla-title\">{0}" + _btnGroupFormat + "</h2>", item.Name, item.ID);

                sbItemHtml.Append("<div class=\"layui-colla-content\">");
                // 注意：如果当前菜单项已经无子项，则为 <p></p>，否则  继续向里递归寻找
                if (item.ChildTargetList == null || item.ChildTargetList.Count == 0)
                {
                    sbItemHtml.AppendFormat("<p>{0} 无子项</p>", item.Name);
                }
                else
                {
                    SubItemList(ref sbItemHtml, item);
                }
                sbItemHtml.Append("</div>");

                sbItemHtml.Append("</div>");
            }
            sbItemHtml.Append("</div>");
        }
        #endregion
    }
}