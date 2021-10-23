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
        private ApplicationDbContext _context = new ApplicationDbContext();

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
        public async Task<ActionResult> Create(CourseCategory courseCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(courseCategory);
            }

            try
            {
                _context.CourseCategories.Add(courseCategory);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                ViewBag.Message = e.InnerException;
                return View(courseCategory);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCategory courseCategory = await _context.CourseCategories.FindAsync(id);
            if (courseCategory == null)
            {
                return HttpNotFound();
            }
            return View(courseCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID, Name, Description")] CourseCategory courseCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(courseCategory).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(courseCategory);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCategory courseCategory = await _context.CourseCategories
                .Include(c => c.Courses)
                .SingleOrDefaultAsync(c => c.Id == id);
            if (courseCategory == null)
            {
                return HttpNotFound();
            }
            return View(courseCategory);
        }


        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCategory courseCategory = await _context.CourseCategories.FindAsync(id);
            if (courseCategory == null)
            {
                return HttpNotFound();
            }
            return View(courseCategory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CourseCategory courseCategory= await _context.CourseCategories.FindAsync(id);
            _context.CourseCategories.Remove(courseCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}