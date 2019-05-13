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
        public ActionResult Index()
        {
            return View();
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

            string name = string.Empty;
            // 如果路径含有~，即需要服务器映射为绝对路径，则进行映射
            basePath = (basePath.IndexOf("~") > -1) ? System.Web.HttpContext.Current.Server.MapPath(basePath) : basePath;
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            // 如果目录不存在，则创建目录
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            string[] suffix = files[0].ContentType.Split('/');
            // 获取文件格式
            string _suffix = suffix[1].Equals("jpeg", StringComparison.CurrentCultureIgnoreCase) ? "" : suffix[1];
            string _temp = System.Web.HttpContext.Current.Request["name"];
            // 如果不修改文件名，则创建随机文件名
            if (!string.IsNullOrEmpty(_temp))
            {
                name = _temp;
            }
            else
            {
                Random rand = new Random(24 * (int)DateTime.Now.Ticks);
                name = rand.Next() + "." + _suffix;
            }
            // 文件保存
            string full = basePath + name;
            files[0].SaveAs(full);
            //string rtnJsonResult = "{\"jsonrpc\" : \"2.0\", \"result\" : null, \"id\" : \"" + name + "\"}";

            FileResult rtnJsonObj = new FileResult
            {
                jsonrpc = "2.0",
                result = null,
                id = name
            };

            // 解压模板文件
            Thread thread = new Thread(() =>
            {
                UnZipTemplate(full);
            })
            { IsBackground = true };
            thread.Start();

            return Json(rtnJsonObj, "text/plain", System.Text.Encoding.UTF8);
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

        #region 解压主题模板Zip
        private void UnZipTemplate(string templateZipPath)
        {
            string sourcePath = templateZipPath;
            string targetPath = Server.MapPath("~/Templates/");

            SharpZip.DecomparessFile(sourcePath, targetPath);
        }
        #endregion
    }
}