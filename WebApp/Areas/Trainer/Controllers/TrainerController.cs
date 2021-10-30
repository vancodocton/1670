using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Utils;

namespace WebApp.Areas.Trainer.Controllers
{
    [Authorize(Roles = Role.Trainer)]
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            string traierId = User.Identity.GetUserId();
            var trainer = await _context.Trainers
                .SingleOrDefaultAsync(t => t.UserId == traierId);
            var courses = _context.Courses
                .Include(c => c.CourseCategory)
                .Where(c => c.Trainers.Any(t => t.UserId == traierId))
                .ToList();
            return View(courses);
        }

        public async Task<ActionResult> ViewTrainee(int? courseId)
        {
            if (courseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            var trainer = await _context.Trainers.SingleOrDefaultAsync(t => t.UserId == userId);
            var course = trainer.Courses.SingleOrDefault(c => c.Id == courseId);

            if (course == null)
            {
                return HttpNotFound();
            }

            var trainees = await _context.Trainees
                .Include(t => t.User)
                .Where(t => t.Courses.Any(c => c.Id == course.Id))
                .ToListAsync();
            return View(trainees);
        }
    }
}