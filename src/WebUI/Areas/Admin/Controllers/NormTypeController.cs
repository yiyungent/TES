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
using WebUI.Areas.Admin.Models.Common;
using WebUI.Areas.Admin.Models.NormTypeVM;
using WebUI.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    public class NormTypeController : Controller
    {
        #region Ctor
        public NormTypeController()
        {
            ViewBag.PageHeader = "评价类型管理";
            ViewBag.PageHeaderDescription = "评价管理";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("业务管理"),
            };
        }
        #endregion

        #region 列表
        public ActionResult Index(int pageIndex = 1, int pageSize = 6)
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            Query(queryConditions);

            ListViewModel<NormType> model = new ListViewModel<NormType>(queryConditions, pageIndex: pageIndex, pageSize: pageSize);
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(model);
        }

        private void Query(IList<ICriterion> queryConditions)
        {
            // 输入的查询关键词
            string query = Request["q"]?.Trim() ?? "";
            // 查询类型
            QueryType queryType = new QueryType();
            queryType.Val = Request["type"]?.Trim() ?? "name";
            switch (queryType.Val.ToLower())
            {
                case "name":
                    queryType.Text = "评价类型名称";
                    queryConditions.Add(Expression.Like("Name", query, MatchMode.Anywhere));
                    break;
                case "id":
                    queryType.Text = "ID";
                    queryConditions.Add(Expression.Eq("ID", int.Parse(query)));
                    break;
                case "normtypecode":
                    queryType.Text = "评价类型代码";
                    queryConditions.Add(Expression.Like("NormTypeCode", query, MatchMode.Anywhere));
                    break;
                default:
                    queryConditions.Add(Expression.Like("Name", query, MatchMode.Anywhere));
                    break;
            }
            ViewBag.Query = query;
            ViewBag.QueryType = queryType;
        }
        #endregion

        #region 删除
        public JsonResult Delete(int id)
        {
            try
            {
                Container.Instance.Resolve<NormTypeService>().Delete(id);

                return Json(new { code = 1, message = "删除成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 1, message = "删除失败" });
            }
        }
        #endregion

        #region 查看
        public ViewResult Detail(int id)
        {
            NormType model = Container.Instance.Resolve<NormTypeService>().GetEntity(id);

            return View(model);
        }
        #endregion

        #region 编辑
        [HttpGet]
        public ViewResult Edit(int id)
        {
            NormType dbModel = Container.Instance.Resolve<NormTypeService>().GetEntity(id);
            NormTypeForEditViewModel viewModel = (NormTypeForEditViewModel)dbModel;

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Edit(NormTypeForEditViewModel inputModel)
        {
            try
            {
                // 数据格式效验
                if (ModelState.IsValid)
                {
                    #region 数据有效效验
                    // 查找 已经有此代码的 (非本正编辑) 的
                    if (Container.Instance.Resolve<NormTypeService>().Exists(inputModel.InputNormTypeCode, inputModel.ID))
                    {
                        return Json(new { code = -3, message = "代码已经存在, 请更换" });
                    }
                    #endregion

                    #region 输入模型 -> 数据库模型
                    NormType dbModel = (NormType)(inputModel);
                    #endregion

                    Container.Instance.Resolve<NormTypeService>().Edit(dbModel);

                    return Json(new { code = 1, message = "保存成功" });
                }
                else
                {
                    string errorMessage = ModelState.GetErrorMessage();
                    return Json(new { code = -1, message = "不合理的输入:" + errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = -2, message = "保存失败" });
            }
        }
        #endregion

        #region 新增
        [HttpGet]
        public ViewResult Create()
        {
            NormTypeForEditViewModel viewModel = new NormTypeForEditViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Create(NormTypeForEditViewModel inputModel)
        {
            try
            {
                // 数据格式效验
                if (ModelState.IsValid)
                {
                    #region 数据有效效验
                    // 查找 已经有此代码的
                    if (Container.Instance.Resolve<NormTypeService>().Exists(inputModel.InputNormTypeCode))
                    {
                        return Json(new { code = -3, message = "代码已经存在, 请更换" });
                    }
                    #endregion

                    #region 输入模型 -> 数据库模型
                    NormType dbModel = (NormType)inputModel;
                    #endregion

                    Container.Instance.Resolve<NormTypeService>().Create(dbModel);

                    return Json(new { code = 1, message = "添加成功" });
                }
                else
                {
                    string errorMessage = ModelState.GetErrorMessage();
                    return Json(new { code = -1, message = "不合理的输入:" + errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = -2, message = "添加失败" });
            }
        }


        #endregion

        #region Helpers



        #endregion
    }
}