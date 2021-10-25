using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Utils;
using WebApp.ViewModels;

namespace WebApp.Areas.Staff.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class CourseCategoryController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var categories = await _context.CourseCategories.ToListAsync();
            return View(categories);
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CourseCategory category)
        {
            if (ModelState.IsValid)
            {
                _context.CourseCategories.Add(category);
                _ = await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
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

            return RedirectToAction("Index");
        }
    }
}