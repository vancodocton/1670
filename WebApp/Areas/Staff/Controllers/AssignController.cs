﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Utils;
using WebApp.ViewModels;
using X.PagedList;

namespace WebApp.Areas.Staff.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class AssignController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        public async Task<ActionResult> Index(int? courseId, int? id)
        {
            if (id != null)
                courseId = id;
            if (courseId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var course = await _context.Courses.SingleOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
                return HttpNotFound();


            var model = new CourseViewModel();

            model.Course = course;

            model.AssignedTrainees = await _context.Trainees
                .Include(t => t.User)
                .Where(t => t.Courses.Any(c => c.Id == courseId))
                .ToListAsync();
            model.AssignedTrainers = await _context.Trainers
                .Include(t => t.User)
                .Where(t => t.Courses.Any(c => c.Id == courseId))
                .ToListAsync();

            return View(model);
        }

        private async Task<AssignViewModel> LoadAssignViewModel(string role)
        {
            var model = new AssignViewModel();

            switch (role)
            {
                case Role.Trainee:
                    model.Trainees = await _context.Trainees
                        .Include(t => t.User)
                        .ToListAsync();
                    break;
                case Role.Trainer:
                    model.Trainers = await _context.Trainers
                        .Include(t => t.User)
                        .ToListAsync();
                    break;
                default:
                    return null;
            }
            model.Role = role;

            return model;
        }

        [HttpGet]
        public async Task<ActionResult> Add(int? courseId, string user)
        {
            if (courseId == null || user == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = await LoadAssignViewModel(user);

            if (model == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AssignViewModel model)
        {
            if (ModelState.IsValid)
            {
                var course = await _context.Courses.SingleOrDefaultAsync(c => c.Id == model.CourseId);
                if (course == null)
                    return HttpNotFound();

                if (model.Role == Role.Trainer)
                {
                    course.Trainers.Add(await _context.Trainers.SingleOrDefaultAsync(t => t.UserId == model.UserId));
                }
                else if (model.Role == Role.Trainee)
                {
                    var trainee = await _context.Trainees.SingleOrDefaultAsync(t => t.UserId == model.UserId);
                    course.Trainees.Add(trainee);                    
                }
                _context.Courses.Attach(course);
                _ = await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { courseId = model.CourseId });
            }

            model = await LoadAssignViewModel(model.Role);

            if (model == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(model);
        }
    }
}
