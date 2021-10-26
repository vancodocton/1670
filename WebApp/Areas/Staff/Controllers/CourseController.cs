using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Utils;
using WebApp.ViewModels;
using X.PagedList;

namespace WebApp.Areas.Staff.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // keyword = course name
        [HttpGet]
        public ActionResult Index(string keyword, int? page)
        {
            var courses = _context.Courses
                .Include(c => c.CourseCategory);

            if (keyword != null)
            {
                keyword = keyword.Trim();
                if (keyword != "")
                    courses = courses.Where(c => c.Name.Contains(keyword.Trim()));
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Course course = await _context.Courses
                .Include(c => c.CourseCategory)
                .SingleOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return HttpNotFound();

            return View(course);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new CreateCourseViewModel()
            {
                Categories = new SelectList(await _context.CourseCategories.ToListAsync(), "Id", "Name")
            };

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var model = new CreateCourseViewModel()
            {
                Categories = new SelectList(await _context.CourseCategories.ToListAsync(), "Id", "Name")
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Course course = await _context.Courses.FindAsync(id);
            if (course == null)
                return HttpNotFound();

            var model = new CreateCourseViewModel()
            {
                Course = course,
                Categories = new SelectList(await _context.CourseCategories.ToListAsync(), "Id", "Name")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CreateCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(model.Course).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.Categories = new SelectList(await _context.CourseCategories.ToListAsync(), "Id", "Name");
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Course course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
