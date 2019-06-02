using Common;
using Core;
using Domain;
using Framework.Common;
using Framework.Mvc.ViewEngines.Templates;
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
        public ActionResult Index(string cat = "open")
        {
            ThemeTemplateViewModel viewModel = new ThemeTemplateViewModel();
            viewModel.List = new List<dynamic>();

            IList<ThemeTemplate> installedTemplateList = Container.Instance.Resolve<ThemeTemplateService>().GetAll();
            IList<string> installedTemplateNames = installedTemplateList.Select(m => m.TemplateName).ToList();
            ITemplateProvider templateProvider = new TemplateProvider(new WebHelper(HttpContext));
            IList<TemplateConfiguration> templateConfigurations = templateProvider.GetTemplateConfigurations();

            switch (cat.ToLower())
            {
                case "open":                // 启用---注意:启用，一定已安装
                    // 数据库中存放的已安装模板 被标记为 启用 的记录
                    IList<string> openTemplateNames = installedTemplateList.Where(m => m.Status == 1).Select(m => m.TemplateName).ToList();
                    foreach (var templateName in openTemplateNames)
                    {
                        OpenCloseItem openItem = new OpenCloseItem();
                        TemplateConfiguration templateConfiguration = templateConfigurations.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).FirstOrDefault();
                        openItem.TemplateName = templateConfiguration.TemplateName;
                        openItem.Title = templateConfiguration.Title;
                        openItem.Authors = templateConfiguration.Authors;
                        openItem.Description = templateConfiguration.Description;
                        openItem.PreviewImageUrl = templateConfiguration.PreviewImageUrl;
                        openItem.IsDefault = installedTemplateList.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).Select(m => m.IsDefault).FirstOrDefault();
                        openItem.Status = installedTemplateList.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).Select(m => m.Status).FirstOrDefault();

                        viewModel.List.Add(openItem);
                    }
                    break;
                case "close":               // 禁用---注意：禁用，一定已安装
                    // 数据库中存放的已安装模板 被标记为 禁用 的记录
                    IList<string> closeTemplateNames = installedTemplateList.Where(m => m.Status == 0).Select(m => m.TemplateName).ToList();
                    foreach (var templateName in closeTemplateNames)
                    {
                        OpenCloseItem openItem = new OpenCloseItem();
                        TemplateConfiguration templateConfiguration = templateConfigurations.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).FirstOrDefault();
                        openItem.TemplateName = templateConfiguration.TemplateName;
                        openItem.Title = templateConfiguration.Title;
                        openItem.Authors = templateConfiguration.Authors;
                        openItem.Description = templateConfiguration.Description;
                        openItem.PreviewImageUrl = templateConfiguration.PreviewImageUrl;
                        openItem.IsDefault = installedTemplateList.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).Select(m => m.IsDefault).FirstOrDefault();
                        openItem.Status = installedTemplateList.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).Select(m => m.Status).FirstOrDefault();

                        viewModel.List.Add(openItem);
                    }
                    break;
                case "installed":           // 已安装
                    // 数据库中存放的已安装模板的记录
                    foreach (var templateName in installedTemplateNames)
                    {
                        OpenCloseItem openItem = new OpenCloseItem();
                        TemplateConfiguration templateConfiguration = templateConfigurations.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).FirstOrDefault();
                        openItem.TemplateName = templateConfiguration.TemplateName;
                        openItem.Title = templateConfiguration.Title;
                        openItem.Authors = templateConfiguration.Authors;
                        openItem.Description = templateConfiguration.Description;
                        openItem.PreviewImageUrl = templateConfiguration.PreviewImageUrl;
                        openItem.IsDefault = installedTemplateList.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).Select(m => m.IsDefault).FirstOrDefault();
                        openItem.Status = installedTemplateList.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).Select(m => m.Status).FirstOrDefault();

                        viewModel.List.Add(openItem);
                    }
                    break;
                case "withoutinstalled":    // 未安装
                    // 在本地检测到的模板安装包，但包名不在 数据库中已安装模板记录中
                    IList<string> zipFilePaths = DetectInstallZip(Server.MapPath(@"~\Upload\TemplateInstallZip"));
                    foreach (string zipFilePath in zipFilePaths)
                    {
                        FileInfo fileInfo = new FileInfo(zipFilePath);
                        string templateName = fileInfo.Name.Remove(fileInfo.Name.LastIndexOf('.'));
                        if (!installedTemplateNames.Contains(templateName, new TemplateNameComparer()))
                        {
                            OpenCloseItem openItem = new OpenCloseItem();
                            TemplateConfiguration templateConfiguration = templateConfigurations.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).FirstOrDefault();
                            openItem.TemplateName = templateConfiguration.TemplateName;
                            openItem.Title = templateConfiguration.Title;
                            openItem.Authors = templateConfiguration.Authors;
                            openItem.Description = templateConfiguration.Description;
                            openItem.PreviewImageUrl = templateConfiguration.PreviewImageUrl;
                            openItem.IsDefault = installedTemplateList.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).Select(m => m.IsDefault).FirstOrDefault();
                            openItem.Status = installedTemplateList.Where(m => m.TemplateName.ToLower() == templateName.ToLower()).Select(m => m.Status).FirstOrDefault();

                            viewModel.List.Add(openItem);
                        }
                    }
                    break;
                default:                    // 启用
                    break;
            }
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




        #region Helpers

        #region 检测模板安装包
        /// <summary>
        /// 检测安装包目录下存在的安装包
        /// </summary>
        /// <param name="installZipDir">安装包目录 ~/Upload/TemplateInstallZip</param>
        /// <returns>返回存在的安装包文件名（包含路径）</returns>
        private IList<string> DetectInstallZip(string installZipDir)
        {
            // 返回 安装包文件名（包括路径）
            IList<string> rtn = new List<string>();
            // 从目录检测存在的模板安装包 (.zip文件)
            string[] installZipFilePaths = Directory.GetFiles(installZipDir, "*.zip");
            foreach (string filePath in installZipFilePaths)
            {
                rtn.Add(filePath);
            }

            return rtn;
        }
        #endregion

        #region 获取文件详细信息
        private FileVersionInfo GetFileVersionInfo(string filePath)
        {
            FileVersionInfo rtn = null;
            if (System.IO.File.Exists(filePath))
            {
                rtn = FileVersionInfo.GetVersionInfo(filePath);
            }
            // 从 FileVersionInfo 中获取文件详细信息，FileInfo 中获取文件大小
            // 参考: https://www.cnblogs.com/shadowme/p/6250036.html

            return rtn;
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

        #endregion
    }

    public class TemplateNameComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            return x.ToLower() == y.ToLower();
        }

        public int GetHashCode(string obj)
        {
            throw new NotImplementedException();
        }
    }
}