using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApp.Models;
using WebApp.Utils;
using WebApp.ViewModels;
using PagedList;
using System.Net;

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

        [HttpGet]
        public async Task<ActionResult> Index(string keyword, int? age, int? page)
        {
            var traineeRole = await _context.Roles.SingleOrDefaultAsync(r => r.Name == Role.Trainee);

            var trainees = _context.Users
                .Where(x => x.Roles.Select(y => y.RoleId).Contains(traineeRole.Id));

            if (keyword != null && keyword.Trim() != "")
            {
                keyword = keyword.Trim().ToLower();
                trainees = trainees.Where(u => u.UserName.ToLower().Contains(keyword));
            }
            if (age != null)
            {
                trainees = trainees.Where(u => u.Age == age);
            }

            trainees = trainees.OrderBy(u => u.Email);

            int pageSize = 3;
            return View(trainees.ToPagedList(page ?? 1, pageSize));
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