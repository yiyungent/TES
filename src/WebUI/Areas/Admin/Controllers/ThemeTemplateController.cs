using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;
using WebUI.Areas.Admin.Models.Template;

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
        public ActionResult Index(string cat = "open")
        {
            IList<ThemeTemplateViewModel> model = new List<ThemeTemplateViewModel>();

            switch (cat.ToLower())
            {
                case "open":                // 启用---注意:启用，一定已安装
                    model.Add(new ThemeTemplateViewModel
                    {
                        Source = Source.Upload,
                        ServerPath = "~/Upload/Templates/Red.zip",
                        TemplateName = "Red",
                        Title = "经典红",
                        Description = "官方推荐主题-经典红",
                        Authors = new List<string> { "TES Office Team" },
                        Url = "",
                        Version = "0.1.0",
                        IsDefault = true,
                        Status = 1
                    });
                    model.Add(new ThemeTemplateViewModel
                    {
                        Source = Source.Upload,
                        ServerPath = "~/Upload/Templates/Red.zip",
                        TemplateName = "Red",
                        Title = "经典红",
                        Description = "官方推荐主题-经典红",
                        Authors = new List<string> { "TES Office Team" },
                        Url = "",
                        Version = "0.1.0",
                        IsDefault = false,
                        Status = 1
                    });
                    model.Add(new ThemeTemplateViewModel
                    {
                        Source = Source.Upload,
                        ServerPath = "~/Upload/Templates/Red.zip",
                        TemplateName = "Red",
                        Title = "经典红",
                        Description = "官方推荐主题-经典红",
                        Authors = new List<string> { "TES Office Team" },
                        Url = "",
                        Version = "0.1.0",
                        IsDefault = false,
                        Status = 1
                    });
                    model.Add(new ThemeTemplateViewModel
                    {
                        Source = Source.Upload,
                        ServerPath = "~/Upload/Templates/Red.zip",
                        TemplateName = "Red",
                        Title = "经典红",
                        Description = "官方推荐主题-经典红",
                        Authors = new List<string> { "TES Office Team" },
                        Url = "",
                        Version = "0.1.0",
                        IsDefault = false,
                        Status = 1
                    });
                    model.Add(new ThemeTemplateViewModel
                    {
                        Source = Source.Upload,
                        ServerPath = "~/Upload/Templates/Red.zip",
                        TemplateName = "Red",
                        Title = "经典红",
                        Description = "官方推荐主题-经典红",
                        Authors = new List<string> { "TES Office Team" },
                        Url = "",
                        Version = "0.1.0",
                        IsDefault = false,
                        Status = 1
                    });
                    break;
                case "close":               // 禁用---注意：禁用，一定已安装
                    break;
                case "installed":           // 已安装
                    break;
                case "withoutinstalled":    // 未安装
                    break;
                default:                    // 启用
                    break;
            }
            ViewBag.Cat = cat;

            return View(model);
        }
        #endregion

        #region 上传本地主题模板
        public ViewResult UploadTemplate()
        {
            return View();
        }

        public JsonResult UploadTemplateFile()
        {
            string basePath = "~/Upload/Templates/";

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
    }
}