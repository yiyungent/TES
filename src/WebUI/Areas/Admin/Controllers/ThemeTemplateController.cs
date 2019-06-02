using Common;
using Core;
using Domain;
using Framework.Common;
using Framework.Mvc.ViewEngines.Templates;
using NHibernate.Criterion;
using Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;
using WebUI.Areas.Admin.Models.ThemeTemplateVM;

namespace WebUI.Areas.Admin.Controllers
{
    public class ThemeTemplateController : Controller
    {
        #region Ctor
        public ThemeTemplateController()
        {
            ViewBag.PageHeader = "主题模板";
            ViewBag.PageHeaderDescription = "主题模板";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("系统管理"),
            };
        }
        #endregion

        #region 主题模板列表
        public ActionResult Index(int pageIndex = 1, int pageSize = 6, string cat = "open")
        {
            IList<ICriterion> queryConditions = new List<ICriterion>();
            ThemeTemplateListViewModel viewModel = new ThemeTemplateListViewModel(queryConditions, pageIndex, pageSize, HttpContext, cat);

            ViewBag.Cat = cat;
            TempData["RedirectUrl"] = Request.RawUrl;

            return View(viewModel);
        }
        #endregion

        #region 上传本地主题模板
        public ViewResult UploadTemplate()
        {
            return View();
        }

        public JsonResult UploadTemplateFile()
        {
            string basePath = "~/Upload/TemplateInstallZip/";

            // 如果路径含有~，即需要服务器映射为绝对路径，则进行映射
            basePath = (basePath.IndexOf("~") > -1) ? System.Web.HttpContext.Current.Server.MapPath(basePath) : basePath;
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            // 如果目录不存在，则创建目录
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            string name = System.Web.HttpContext.Current.Request["name"];
            // 文件保存
            string fullPath = basePath + name;
            files[0].SaveAs(fullPath);

            FileResult rtnJsonObj = new FileResult
            {
                jsonrpc = "2.0",
                result = null,
                id = name
            };

            return Json(rtnJsonObj);
        }
        #endregion

        #region 启用禁用模板
        public JsonResult OpenClose(int id)
        {
            try
            {
                if (!Container.Instance.Resolve<ThemeTemplateService>().Exist(id))
                {
                    return Json(new { code = -1, message = "指定的模板不存在" });
                }
                ThemeTemplate dbModel = Container.Instance.Resolve<ThemeTemplateService>().GetEntity(id);
                string msg = "";
                switch (dbModel.Status)
                {
                    case 0:
                        msg = "启用";
                        dbModel.Status = 1;
                        break;
                    case 1:
                        dbModel.Status = 0;
                        msg = "禁用";
                        break;
                }
                Container.Instance.Resolve<ThemeTemplateService>().Edit(dbModel);

                return Json(new { code = 1, message = msg + "成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = "切换失败" });
            }
        }
        #endregion


        #region Helpers

        #region 安装本地主题模板
        /// <summary>
        /// 安装本地主题模板
        /// </summary>
        /// <param name="templateServerPath">eg. ~/Upload/Templates/Red.zip</param>
        /// <returns></returns>
        public async Task<JsonResult> InstallLocationTemplate(string templateServerPath)
        {
            string fullPhysicalPath = System.Web.HttpContext.Current.Server.MapPath(templateServerPath);
            await Task.Run(() =>
            {
                // 解压上传的模板包 到 视图地 ~/Templates
                UnZipTemplate(fullPhysicalPath);
                // 将模板信息 插入数据库，默认安装完后为启用
            });

            return Json(new { code = 1, message = "安装完成" });
        }
        #endregion

        #region 解压主题模板
        private void UnZipTemplate(string templateZipPath)
        {
            string sourcePath = templateZipPath;
            string targetPath = Server.MapPath("~/Templates/");

            SharpZip.DecomparessFile(sourcePath, targetPath);
        }
        #endregion

        #region 上传文件json结果
        sealed class FileResult
        {
            public string jsonrpc { get; set; }

            public string result { get; set; }

            public string id { get; set; }
        }
        #endregion 

        #endregion
    }
}