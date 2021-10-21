﻿using System;
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

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CourseCategoryId = new SelectList(_context.CourseCategories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,CourseCategoryId,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CourseCategoryId = new SelectList(_context.CourseCategories, "Id", "Name", course.CourseCategoryId);
            return View(course);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
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
            ViewBag.CourseCategoryId = new SelectList(_context.CourseCategories, "Id", "Name", course.CourseCategoryId);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,CourseCategoryId,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(course).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CourseCategoryId = new SelectList(_context.CourseCategories, "Id", "Name", course.CourseCategoryId);
            return View(course);
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

        [HttpGet]
        public async Task<ActionResult> Assign(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Trainees)
                .Include(c => c.Trainers)
                .SingleOrDefaultAsync(c => c.Id == id);

            var model = new AssignViewModel
            {
                Course = course,
                Trainees = await _context.Trainees
                    .Include(t => t.User)
                    .ToListAsync(),
                Trainers = await _context.Trainers
                    .Include(t => t.User)
                    .ToListAsync(),
                AssignedTrainers = await course.Trainers.ToListAsync(),
                AssignedTrainees = await course.Trainees.ToListAsync()
            };

            return View(model);
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
            if (model.TraineeId != null)
            {
                var trainee = await _context.Trainees.SingleOrDefaultAsync(t => t.UserId == model.TraineeId);
                course.Trainees.Add(trainee);
                _context.Courses.Attach(course);
                _ = await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Assign), new { id = model.Course.Id });
        }
    }
}
