using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Areas.Trainer.Controllers
{
    public class HomeController : Controller
    {
        // GET: Trainer/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}