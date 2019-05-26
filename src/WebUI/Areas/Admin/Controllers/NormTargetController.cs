using Core;
using Domain;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Admin.Controllers
{
    public class NormTargetController : Controller
    {
        #region Ctor
        public NormTargetController()
        {
            ViewBag.PageHeader = "指标管理";
            ViewBag.PageHeaderDescription = "指标管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("评价管理"),
            };
        }
        #endregion

        #region 列表
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 编辑
        [HttpGet]
        public ViewResult Edit(int id)
        {
            NormTarget viewModel = Container.Instance.Resolve<NormTargetService>().GetEntity(id);
            int parentId = viewModel.ParentTarget?.ID ?? 0;
            ViewBag.DDLParent = InitDDLForParent(viewModel, parentId);
            //ViewBag.DDLNormType = Init

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Edit(NormTarget inputModel)
        {
            try
            {
                NormTarget dbModel = Container.Instance.Resolve<NormTargetService>().GetEntity(inputModel.ID);
                // 上级
                if (inputModel.ParentTarget.ID == 0)
                {
                    inputModel.ParentTarget = null;
                }
                // 设置修改后的值
                dbModel.Name = inputModel.Name;
                dbModel.SortCode = inputModel.SortCode;
                dbModel.ParentTarget = inputModel.ParentTarget;
                //dbModel.NormType = inputModel.NormType;

                Container.Instance.Resolve<NormTargetService>().Edit(dbModel);

                return Json(new { code = 1, message = "保存成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "保存失败" });
            }
        }
        #endregion

        #region Helpers
        private IList<SelectListItem> InitDDLForParent(NormTarget self, int parentId)
        {
            IList<SelectListItem> ret = new List<SelectListItem>();
            ret.Add(new SelectListItem()
            {
                Text = "请选择(一级)",
                Value = "0",
                Selected = (0 == parentId),
            });

            IList<NormTarget> all = Container.Instance.Resolve<NormTargetService>().GetAll();
            // 找出自己及后代的ID
            IList<int> idRange = new List<int>();
            GetIdRange(self, idRange, all);

            // not in 查询
            var find = from m in all
                       where idRange.Contains(m.ID) == false
                       select m;
            // 方案一：不考虑层级的实现
            //foreach (var item in find)
            //{
            //    ret.Add(new SelectListItem()
            //    {
            //        Text = item.Name,
            //        Value = item.ID.ToString(),
            //        Selected = (item.ID == parentId)
            //    });
            //}
            // 方案二：考虑层级的实现
            // 1.一级菜单
            var first = from m in find
                        where m.ParentTarget == null
                        orderby m.SortCode
                        select m;
            foreach (var item in first)
            {
                AddSelfAndChildrenForDDL("", item, ret, find.ToList(), parentId);
            }

            return ret;
        }

        private void AddSelfAndChildrenForDDL(string pref, NormTarget self, IList<SelectListItem> target, IList<NormTarget> all, int parentId)
        {
            // 1.添加自己
            target.Add(new SelectListItem()
            {
                Text = pref + self.Name,
                Value = self.ID.ToString(),
                Selected = (self.ID == parentId)
            });
            // 2.递归添加子女
            var child = from m in all
                        where m.ParentTarget != null && m.ParentTarget.ID == self.ID
                        orderby m.SortCode
                        select m;
            foreach (var item in child)
            {
                AddSelfAndChildrenForDDL(pref + "--", item, target, all, parentId);
            }
        }

        /// <summary>
        /// 递归方法：添加自己及其子女
        /// </summary>
        /// <param name="self"></param>
        /// <param name="idRange"></param>
        /// <param name="all"></param>
        private void GetIdRange(NormTarget self, IList<int> idRange, IList<NormTarget> all)
        {
            // 添加自己
            idRange.Add(self.ID);

            // 关于子女循环
            // 第二种解决方法：
            if (self.ChildTargetList == null) return;
            foreach (var item in self.ChildTargetList)
            {
                // 递归调用---添加自己和自己的子女
                GetIdRange(item, idRange, all);
            }
        }
        #endregion



    }
}