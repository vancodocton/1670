using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Areas.Staff.Data;
using WebApp.Models;
using WebApp.Models.Profiles;
using WebApp.Utils;
using WebApp.ViewModels;
using X.PagedList;

namespace WebApp.Areas.Staff.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class AssignController : BaseAccountController
    {
        protected override void SetManagedRoles()
        {
            _managedRoles.Add(Role.Trainer);
            _managedRoles.Add(Role.Trainee);
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

        [HttpPost]
        private async Task<AssignViewModel> LoadAssignViewModel(string role, AssignViewModel model = null)
        {
            model = model ?? new AssignViewModel();
            model.Role = role;

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

            return model;
        }

        [HttpGet]
        public async Task<ActionResult> Add(int? courseId, string userRole)
        {
            if (courseId == null || userRole == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var course = await _context.Courses.SingleOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var model = await LoadAssignViewModel(userRole);

            if (model == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            model.CourseId = course.Id;
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
                    var trainer = await _context.Trainers.SingleOrDefaultAsync(t => t.UserId == model.UserId);
                    course.Trainers.Add(trainer);
                }
                else if (model.Role == Role.Trainee)
                {
                    var trainee = await _context.Trainees.SingleOrDefaultAsync(t => t.UserId == model.UserId);
                    course.Trainees.Add(trainee);
                }
                _context.Courses.Attach(course);
                int row = await _context.SaveChangesAsync();

                if (row == 0)
                {
                    ModelState.AddModelError("", string.Format("The {0} has assigned to the course", model.Role));

                    model = await LoadAssignViewModel(model.Role, model);
                    return View(model);
                }

                return RedirectToAction(nameof(Index), new { courseId = model.CourseId });
            }

            model = await LoadAssignViewModel(model.Role);

            if (model == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Remove(int? courseId, string userRole, string userId, bool saveChangesError = false)
        {
            // validate parameters
            if (courseId == null || userRole == null || userId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var course = await _context.Courses
                .SingleOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                return HttpNotFound();

            var model = new UserProfileViewModel();
            switch (userRole)
            {
                case Role.Trainee:
                    if (course.Trainees.Any(t => t.UserId == userId))
                    {
                        model = await GetUserViewModel(userId);
                    }
                    break;
                case Role.Trainer:
                    if (course.Trainers.Any(t => t.UserId == userId))
                    {
                        model = await GetUserViewModel(userId);
                    }
                    break;
                default:
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError)
            {
                ModelState.AddModelError("",
                    "Delete failed.Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmRemove(int? courseId, string userRole, string userId)
        {
            // validate parameters
            if (courseId == null || userRole == null || userId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var course = await _context.Courses
                .SingleOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                return HttpNotFound();

            var model = new UserProfileViewModel();
            switch (userRole)
            {
                case Role.Trainee:
                    var trainee = course.Trainees.SingleOrDefault(t => t.UserId == userId);
                    course.Trainees.Remove(trainee);
                    break;
                case Role.Trainer:
                    var trainer = course.Trainers.SingleOrDefault(t => t.UserId == userId);
                    course.Trainers.Remove(trainer);
                    break;
                default:
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _context.Courses.Attach(course);

            _ = await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { courseId });
        }

        // search enrolled trainer/trainee by course name
        [HttpGet]
        public async Task<ActionResult> Search(string userRole, string keyword)
        {
            var model = new AssignedSearchVewModel();

            //model.Roles = new List<string>() { Role.Trainee, Role.Trainer };
            model.Roles = _managedRoles;
            if (keyword == null)
                return View(model);

            keyword = keyword.Trim().ToLower();

            if (_managedRoles.Contains(userRole))
            {
                model.UserRole = userRole;
                var courses = _context.Courses
                    .Where(c =>
                        c.Name
                        .ToLower()
                        .Contains(keyword));

                courses = courses.OrderBy(c => c.Id);

                switch (userRole)
                {
                    case Role.Trainee:
                        model.GroupedUsers = await courses
                            .Include(c => c.Trainees)
                            .Select(c => new GroupedUsersViewModel<ApplicationUser>()
                            {
                                Type = c.Name,
                                Users = c.Trainees.Select(t => t.User).ToList()
                            })
                            .ToListAsync();
                        break;
                    case Role.Trainer:
                        model.GroupedUsers = await courses
                            .Include(c => c.Trainers)
                            .Select(c => new GroupedUsersViewModel<ApplicationUser>()
                            {
                                Type = c.Name,
                                Users = c.Trainers.Select(t => t.User).ToList()
                            })
                            .ToListAsync();
                        break;
                        //default:
                        //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        //    return View();
                }
            }

            return View(model);
        }
    }
}
