using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Utils;
using X.PagedList;
using WebApp.ViewModels;

namespace WebApp.Areas.Trainer.Controllers
{
    public class TrainerViewCourseController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        [HttpGet]
        public ActionResult Index(string CourseName, int? page)
        {
            var courses = _context.Courses
                .Include(c => c.CourseCategory);

            if (CourseName != null)
            {
                CourseName = CourseName.Trim();
                if (CourseName != "")
                    courses = courses.Where(c => c.Name.Contains(CourseName.Trim()));
            }

            courses = courses.OrderBy(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(courses.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await _context.Courses
                .Include(c => c.CourseCategory)
                .SingleOrDefaultAsync(c => c.Id == id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Assign(AssignViewModel model)
        {
            var course = await _context.Courses
                .SingleOrDefaultAsync(c => c.Id == model.Course.Id);

            if (model.TrainerId != null)
            {
                course.Trainers.Add(await _context.Trainers.SingleOrDefaultAsync(t => t.UserId == model.TrainerId));
                _context.Courses.Attach(course);
                _ = await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Assign), new { id = model.Course.Id });
        }
    }
}