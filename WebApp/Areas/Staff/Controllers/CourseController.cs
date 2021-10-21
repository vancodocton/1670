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

namespace WebApp.Areas.Staff.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index(string CourseName, int? page)
        {
            var courses = db.Courses
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
            Course course = await db.Courses
                .Include(c => c.CourseCategory)
                .SingleOrDefaultAsync(c => c.Id == id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CourseCategoryId = new SelectList(db.CourseCategories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,CourseCategoryId,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CourseCategoryId = new SelectList(db.CourseCategories, "Id", "Name", course.CourseCategoryId);
            return View(course);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseCategoryId = new SelectList(db.CourseCategories, "Id", "Name", course.CourseCategoryId);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,CourseCategoryId,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CourseCategoryId = new SelectList(db.CourseCategories, "Id", "Name", course.CourseCategoryId);
            return View(course);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
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
            Course course = await db.Courses.FindAsync(id);
            db.Courses.Remove(course);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public async Task<ActionResult> Assign(int id)
        {
            var course = await db.Courses.Include(c => c.CourseTrainees).SingleOrDefaultAsync(c => c.Id == id);
            var model = new AssignViewModel();

            model.Course = course;

            var arr = course.CourseTrainees.Select(ct => ct.TraineeUserId);

            //var a = db.Courses
            //    .GroupJoin(db.CourseTrainees, c => c.Id, ct => ct.CourseId, (c, ct) =>
            //    ct.Select(i => i.TraineeProfile));

            var trainees = db.CourseTrainees
                .Include(ct => ct.TraineeProfile.User)
                .Where(ct => ct.CourseId == id);

            model.AssignedTrainees = trainees
                .Select(ct => ct.TraineeProfile)
                .Include(p => p.User)
                .ToList();
            

            return View(model);
        }
    }
}
