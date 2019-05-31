using Core;
using Domain;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Admin.Controllers
{
    public class DepartmentController : Controller
    {
        #region Ctor
        public DepartmentController()
        {
            ViewBag.PageHeader = "部门管理";
            ViewBag.PageHeaderDescription = "部门管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
            };
        }
        #endregion

        #region 列表
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 排序
        /// <summary>
        /// 对子项排序
        /// </summary>
        /// <param name="id">父项ID</param>
        /// <returns></returns>
        [HttpGet]
        public ViewResult Sort(int id)
        {
            IList<Department> viewModel = null;
            if (id == 0)
            {
                viewModel = Container.Instance.Resolve<DepartmentService>().Query(new List<ICriterion>
                {
                    Expression.IsNull("ParentDept.ID")
                }).OrderBy(m => m.SortCode).ToList();
            }
            else
            {
                viewModel = Container.Instance.Resolve<DepartmentService>().Query(new List<ICriterion>
                {
                    Expression.Eq("ParentDept.ID", id)
                }).OrderBy(m => m.SortCode).ToList();
            }

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Sort(string ids)
        {
            try
            {
                string[] idArr = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                Department item;
                for (int i = 0; i < idArr.Length; i++)
                {
                    item = Container.Instance.Resolve<DepartmentService>().GetEntity(int.Parse(idArr[i]));
                    if (item != null)
                    {
                        item.SortCode = (i + 1) * 10;
                        Container.Instance.Resolve<DepartmentService>().Edit(item);
                    }
                }

                return Json(new { code = 1, message = "保存成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "保存失败" });
            }
        }
        #endregion

        #region 编辑
        [HttpGet]
        public ViewResult Edit(int id)
        {
            Department viewModel = Container.Instance.Resolve<DepartmentService>().GetEntity(id);
            int parentId = viewModel.ParentDept?.ID ?? 0;
            ViewBag.DDLParent = InitDDLForParent(viewModel, parentId);

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Edit(Department inputModel)
        {
            try
            {
                Department dbModel = Container.Instance.Resolve<DepartmentService>().GetEntity(inputModel.ID);
                // 上级
                if (inputModel.ParentDept.ID == 0)
                {
                    inputModel.ParentDept = null;
                }
                // 设置修改后的值
                dbModel.Name = inputModel.Name;
                dbModel.SortCode = inputModel.SortCode;
                dbModel.ParentDept = inputModel.ParentDept;

                Container.Instance.Resolve<DepartmentService>().Edit(dbModel);

                return Json(new { code = 1, message = "保存成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "保存失败" });
            }
        }
        #endregion

        #region 删除
        public JsonResult Delete(int id)
        {
            try
            {
                Container.Instance.Resolve<DepartmentService>().Delete(id);

                return Json(new { code = 1, message = "删除成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "删除失败" });
            }
        }
        #endregion

        #region 新增
        [HttpGet]
        public ViewResult Create()
        {
            Department viewModel = new Department();
            ViewBag.DDLParent = InitDDLForParent(viewModel, 0);

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Create(Department inputModel)
        {
            try
            {
                // 上级
                if (inputModel.ParentDept.ID == 0)
                {
                    inputModel.ParentDept = null;
                }

                // 设置修改后的值
                inputModel.Name = inputModel.Name;
                inputModel.SortCode = inputModel.SortCode;
                inputModel.ParentDept = inputModel.ParentDept;

                Container.Instance.Resolve<DepartmentService>().Create(inputModel);

                return Json(new { code = 1, message = "添加成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "添加失败" });
            }
        }
        #endregion

        #region Helpers

        private IList<SelectListItem> InitDDLForParent(Department self, int parentId)
        {
            IList<SelectListItem> ret = new List<SelectListItem>();
            ret.Add(new SelectListItem()
            {
                Text = "请选择(一级)",
                Value = "0",
                Selected = (0 == parentId),
            });

            IList<Department> all = Container.Instance.Resolve<DepartmentService>().GetAll();
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
                        where m.ParentDept == null
                        orderby m.SortCode
                        select m;
            foreach (var item in first)
            {
                AddSelfAndChildrenForDDL("", item, ret, find.ToList(), parentId);
            }

            return ret;
        }

        private void AddSelfAndChildrenForDDL(string pref, Department self, IList<SelectListItem> target, IList<Department> all, int parentId)
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
                        where m.ParentDept != null && m.ParentDept.ID == self.ID
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
        private void GetIdRange(Department self, IList<int> idRange, IList<Department> all)
        {
            // 添加自己
            idRange.Add(self.ID);

            // 关于子女循环
            // 第二种解决方法：
            if (self.Children == null) return;
            foreach (var item in self.Children)
            {
                // 递归调用---添加自己和自己的子女
                GetIdRange(item, idRange, all);
            }
        }

        #endregion
    }
}