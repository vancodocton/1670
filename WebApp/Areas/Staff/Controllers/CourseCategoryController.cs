using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Utils;

namespace WebApp.Areas.Staff.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class CourseCategoryController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        [HttpGet]
        public async Task<ActionResult> Index(string keyword)
        {
            var categories = _context.CourseCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();

                categories = categories.Where(c => c.Name.Contains(keyword));
            }
            categories = categories.OrderBy(c => c.Name);

            return View(await categories.ToListAsync());
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CourseCategory category)
        {
            if (ModelState.IsValid)
            {
                if (await _context.CourseCategories.AnyAsync(c => c.Name == category.Name))
                {
                    ModelState.AddModelError("", "There has a category named '" + category.Name + "' already.");

                }
                else
                {
                    _context.CourseCategories.Add(category);
                    _ = await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            return View(category);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            CourseCategory courseCategory = await _context.CourseCategories.FindAsync(id);
            if (courseCategory == null)
                return HttpNotFound("No course category found");

            return View(courseCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CourseCategory category)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(category).State = EntityState.Modified;
                _ = await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            CourseCategory category = await _context.CourseCategories
                .Include(c => c.Courses)
                .SingleOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return HttpNotFound("No course category found");

            return View(category);
        }


        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            CourseCategory category = await _context.CourseCategories.FindAsync(id);
            if (category == null)
                return HttpNotFound("No course category found");

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CourseCategory category = await _context.CourseCategories.FindAsync(id);
            if (category == null)
                return RedirectToAction(nameof(Index));

            _context.CourseCategories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}