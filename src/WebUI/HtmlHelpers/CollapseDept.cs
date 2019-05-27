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
    public static class CollapseDept
    {
        private static IList<Department> _allList;

        private const string _btnGroupFormat = "<div class='layui-btn-group' style='float: right;'><button class='layui-btn layui-btn-sm' onclick='onEdit({1})'>修改</button></div>";

        #region 产生菜单列表首页
        public static MvcHtmlString GenerateCollapseDept(this HtmlHelper value)
        {
            IList<Department> allDeptList = Container.Instance.Resolve<DepartmentService>().GetAll();
            _allList = allDeptList;
            IList<Department> firstDeptList = (from m in allDeptList
                                               where m.ParentDept == null
                                               orderby m.SortCode ascending
                                               select m).ToList();

            StringBuilder sbMenuHtml = new StringBuilder();
            // 折叠菜单 layui-collapse
            sbMenuHtml.Append("<div class=\"layui-collapse\" lay-accordion=\"\">");

            // 当前同等级别(一级菜单项) 的  菜单项(许多)
            foreach (var firstDeptItem in firstDeptList)
            {
                // 折叠菜单项 layui-colla-item
                sbMenuHtml.Append("<div class=\"layui-colla-item\">");
                // 此菜单项标题 layui-colla-title
                sbMenuHtml.AppendFormat("<h2 class=\"layui-colla-title\">{0}" + _btnGroupFormat + "</h2>", firstDeptItem.Name, firstDeptItem.ID);

                // 此菜单项内容:  1. 无子项---<p></p>   2. 有子项---折叠菜单
                sbMenuHtml.Append("<div class=\"layui-colla-content\">");
                // 里面又有许多项----此处开始进入 首次递归----------直到最后某项不再是任何项的父亲，则为 <p></p>
                // 深度优先--- 利用循环拿到每个菜单项,再进递归拿取其子菜单
                // 注意：如果当前菜单项已经无子项，则为 <p></p>，否则  继续向里递归寻找
                if (firstDeptItem.Children == null || firstDeptItem.Children.Count == 0)
                {
                    sbMenuHtml.AppendFormat("<p>{0} </p>", firstDeptItem.Name);
                }
                else
                {
                    // 否则又是一个折叠菜单
                    SubDeptList(ref sbMenuHtml, firstDeptItem);
                }

                sbMenuHtml.Append("</div>");

                sbMenuHtml.Append("</div>");
            }

            sbMenuHtml.Append("</div>");

            return MvcHtmlString.Create(sbMenuHtml.ToString());
        }
        #endregion

        #region 递归查找子菜单项
        private static void SubDeptList(ref StringBuilder sbDeptHtml, Department currentDept)
        {
            IList<Department> suDeptList = (from m in _allList
                                            where m.ParentDept != null && m.ParentDept.ID == currentDept.ID
                                            orderby m.SortCode ascending
                                            select m).ToList();
            // 此菜单项下又嵌套一个折叠菜单
            sbDeptHtml.Append("<div class=\"layui-collapse\" lay-accordion=\"\">");
            foreach (var deptItem in suDeptList)
            {
                sbDeptHtml.Append("<div class=\"layui-colla-item\">");
                sbDeptHtml.AppendFormat("<h2 class=\"layui-colla-title\">{0}" + _btnGroupFormat + "</h2>", deptItem.Name, deptItem.ID);

                sbDeptHtml.Append("<div class=\"layui-colla-content\">");
                // 注意：如果当前菜单项已经无子项，则为 <p></p>，否则  继续向里递归寻找
                if (deptItem.Children == null || deptItem.Children.Count == 0)
                {
                    sbDeptHtml.AppendFormat("<p>{0} </p>", deptItem.Name);
                }
                else
                {
                    SubDeptList(ref sbDeptHtml, deptItem);
                }
                sbDeptHtml.Append("</div>");

                sbDeptHtml.Append("</div>");
            }
            sbDeptHtml.Append("</div>");
        }
        #endregion
    }
}