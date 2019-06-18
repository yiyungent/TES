using Core;
using Domain;
using RankingChart.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RankingChart.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View("~/Plugins/RankingChart/Views/Home/Index.cshtml");
        }

        public JsonResult GetData()
        {
            EvaResultService evaResultService = Container.Instance.Resolve<EvaResultService>();
            IList<EvaResult> allEvaResult = evaResultService.GetAll();
            allEvaResult = allEvaResult.OrderBy(m => m.EvaluateTask.EndDate).ToList();
            IList<JsonDataModel> jsonArr = new List<JsonDataModel>();
            foreach (var item in allEvaResult)
            {
                jsonArr.Add(new JsonDataModel
                {
                    name = $"{item.Teacher.Name}({item.Teacher.EmployeeCode})",
                    type = item.NormType.Name,
                    value = item.Score.ToString("#.##"),
                    date = item.EvaluateTask.EndDate.ToString("yyyy-MM-dd")
                });
            }

            return Json(jsonArr, JsonRequestBehavior.AllowGet);
        }
    }
}