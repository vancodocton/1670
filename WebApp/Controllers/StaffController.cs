using System.Web.Mvc;
using WebApp.Utils;

namespace WebApp.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class StaffController : Controller
    {
        // GET: Staff
        public ActionResult Index()
        {
            return View();
        }
    }
}