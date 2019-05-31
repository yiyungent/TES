using Core;
using Domain;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Admin.Models;
using WebUI.Areas.Admin.Models.DashboardVM;

namespace WebUI.Areas.Admin.Controllers
{
    /// <summary>
    /// 仪表盘
    /// </summary>
    public class DashboardController : Controller
    {
        #region Ctor
        public DashboardController()
        {
            ViewBag.PageHeader = "评价分析";
            ViewBag.PageHeaderDescription = "评价分析";
            ViewBag.BreadcrumbList = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("仪表盘"),
                new BreadcrumbItem("评价分析"),
            };
        }
        #endregion

        #region 评价分析
        [HttpGet]
        public ViewResult EvaAnalyze()
        {
            return View();
        }

        [HttpPost]
        public JsonResult EvaAnalyze(bool flag = false)
        {
            EvaAnalyzeChart chartOption = new EvaAnalyzeChart();

            IList<EvaTask> evaTaskList = Container.Instance.Resolve<EvaTaskService>().GetAll();
            chartOption.legend = new Legend() { data = new string[evaTaskList.Count] };
            for (int i = 0; i < evaTaskList.Count; i++)
            {
                chartOption.legend.data[i] = evaTaskList[i].Name;
            }
            chartOption.xAxis = new Xaxi[1];
            chartOption.xAxis[0] = new Xaxi();
            chartOption.xAxis[0].type = "category";
            chartOption.xAxis[0].boundaryGap = false;
            chartOption.xAxis[0].data = new string[7] { "一", "二", "三", "周四", "周五", "周六", "周日" };
            chartOption.yAxis = new Yaxi[1];
            chartOption.yAxis[0] = new Yaxi();
            chartOption.yAxis[0].type = "value";


            return Json(chartOption);
        }
        #endregion
    }
}