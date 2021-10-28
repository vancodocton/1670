using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Utils;

namespace WebApp.Areas.Trainee.Controllers
{
    [Authorize(Roles = Role.Trainee)]
    public class TraineeController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            string traineeId = User.Identity.GetUserId();

            var trainee = await _context.Users.SingleOrDefaultAsync(t => t.Id == traineeId);

            var courses = await _context.Courses
                .Include(t => t.CourseCategory)
                .Where(t => t.Trainees.Any(c => c.UserId == traineeId))
                .ToListAsync();

            return View(courses);
        }

        public async Task<ActionResult> ViewTrainee(int courseId)
        {
            var userId = User.Identity.GetUserId();
            var trainee = await _context.Trainees.SingleOrDefaultAsync(t => t.UserId == userId);

            var course = trainee.Courses.SingleOrDefault(c => c.Id == courseId);
            // check if user had enrolled to course
            if (course == null)
                return HttpNotFound();

            //load trainees in course
            var trainees = await _context.Trainees
                .Include(t => t.User)
                .Where(t => t.Courses.Any(c => c.Id == courseId))
                .ToListAsync();

            // remove user from course's trainees
            trainees.Remove(trainee);

            // pass Course Name to View by ViewBag
            ViewBag.CourseName = course.Name;

            return View(trainees);
        }
    }
}