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
        public async Task<ActionResult> Index(string keyword, int? page)
        {
            var courses = _context.Courses
                .AsQueryable()
                .Include(c => c.CourseCategory);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                courses = courses.Where(c => c.Name.Contains(keyword.Trim()));
            }

            courses = courses.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(await courses.ToPagedListAsync(pageNumber, pageSize));
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
                if (await _context.Courses.AnyAsync(c => c.Name == course.Name))
                {
                    ModelState.AddModelError("", "There has a course named '" + course.Name + "' already.");
                }
                else
                {
                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
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
                if (await _context.Courses.AnyAsync(c => c.Id != model.Course.Id && c.Name == model.Course.Name))
                {
                    ModelState.AddModelError("", "There has a course named '" + model.Course.Name + "' already.");
                }
                else
                {
                    _context.Entry(model.Course).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { model.Course.Id });
                }
            }

            model.Categories = new SelectList(await _context.CourseCategories.ToListAsync(), "Id", "Name");
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
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

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Course course = await _context.Courses
                .FindAsync(id);

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
