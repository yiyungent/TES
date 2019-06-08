using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Plugin.Contents.Controllers
{
    public class ShowContentsController : Controller
    {
        // GET: ShowContents
        public ActionResult Index()
        {
            return View();
        }
    }
}