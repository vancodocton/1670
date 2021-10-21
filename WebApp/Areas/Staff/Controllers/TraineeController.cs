using System;
using System.Collections.Generic;
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
    public class TraineeController : BaseAccountController
    {
        public TraineeController() : base() { }

        public TraineeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager) { }


        protected override void SetManagedRoles()
        {
            roles.Add(Role.Trainee);
        }

        protected async Task<IPagedList<ApplicationUser>> GetPagedNames(IQueryable<ApplicationUser> users, int page)
        {
            const int pageSize = 3;

            if (page < 1)
                return null;

            var listPaged = await users.ToPagedListAsync(page, pageSize);

            if (listPaged.PageNumber != 1 && page > listPaged.PageCount)
                return null;

            return listPaged;
        }


        [HttpGet]
        public async Task<ActionResult> Index(string name, int? age, int? page)
        {
            var traineeRole = await RoleManager.FindByNameAsync(Role.Trainee);

            var trainees = _context.Users
                .Include(u => u.Trainee)
                .Where(x => x.Roles
                    .All(r => r.RoleId == traineeRole.Id));

            if (name != null)
            {
                name = name.Trim().ToLower();
                if (name != "")
                    trainees = trainees.Where(u => u.UserName.ToLower().Contains(name));
            }

            if (age != null)
            {
                trainees = trainees.Where(u => u.Age == age);
            }

            trainees = trainees.OrderBy(u => u.Email);

            var listPaged = await GetPagedNames(trainees, page ?? 1);

            if (listPaged == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(listPaged);
        }

        protected override async Task<UserViewModel> LoadUserViewModel(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            var roles = await UserManager.GetRolesAsync(user.Id);
            if (!roles.All(r => r == Role.Trainee))
                return null;

            var model = new UserViewModel()
            {
                User = user,
                Roles = new List<string>(roles)
            };

            model = await LoadUserProfile(model);

            return model;
        }

        protected override async Task<UserViewModel> LoadUserProfile(UserViewModel model)
        {
            var trainee = await _context.Trainees.SingleOrDefaultAsync(u => u.UserId == model.User.Id);

            if (trainee == null)
            {
                trainee = new Models.Trainee()
                {
                    UserId = model.User.Id,
                    Education = null,
                    BirthDate = null,
                };

                _context.Trainees.Add(trainee);
                await _context.SaveChangesAsync();
            }

            model.Education = trainee.Education;
            model.BirthDate = trainee.BirthDate;

            return model;
        }

        protected override async Task<UserViewModel> UpdateUserProfile(UserViewModel model)
        {
            if (roles.All(r => r == Role.Trainee))
            {
                var trainee = await _context.Trainees.SingleOrDefaultAsync(u => u.UserId == model.User.Id);

                if (trainee == null)
                {
                    trainee = new Models.Trainee()
                    {
                        UserId = model.User.Id,
                        Education = null,
                        BirthDate = null,
                    };
                    _context.Trainees.Add(trainee);
                }
                else
                {
                    trainee.Education = model.Education;
                    trainee.BirthDate = model.BirthDate;
                }

                await _context.SaveChangesAsync();
                return model;
            }

            return null;
        }
    }
}