using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Areas.Train.Controllers
{
    public class HomeController : Controller
    {
        // GET: Train/Default
        public ActionResult Index()
        {
            return View();
        }
    }
}