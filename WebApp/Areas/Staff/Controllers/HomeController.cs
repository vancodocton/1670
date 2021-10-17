using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Utils;

namespace WebApp.Areas.Staff.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class HomeController : Controller
    {
        // GET: Staff/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}