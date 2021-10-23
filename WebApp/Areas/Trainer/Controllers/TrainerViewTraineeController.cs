using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Areas.Trainer.Controllers
{
    public class TrainerViewTraineeController : Controller
    {
        // GET: Trainer/TrainerViewTrainee
        public ActionResult Index()
        {
            return View();
        }
    }
}