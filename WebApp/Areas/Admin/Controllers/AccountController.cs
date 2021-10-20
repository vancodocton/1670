using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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

namespace WebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class AccountController : BaseAccountController
    {
        public AccountController() : base() { }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager) { }

        protected override void SetManagedRoles()
        {
            roles.Add(Role.Staff);
            roles.Add(Role.Trainer);
        }

        protected override async Task<UserViewModel> LoadUserViewModel(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            var roles = await UserManager.GetRolesAsync(user.Id);

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
            if (roles.Contains(Role.Trainer))
            {
                var trainer = await _context.Trainers.SingleOrDefaultAsync(u => u.UserId == model.User.Id);

                if (trainer == null)
                {
                    trainer = new Models.Trainer()
                    {
                        UserId = model.User.Id,
                        Specialty = null
                    };

                    _context.Trainers.Add(trainer);
                    await _context.SaveChangesAsync();
                }
                model.Specialty = trainer.Specialty;
            }

            return model;
        }

        protected override async Task<UserViewModel> UpdateUserProfile(UserViewModel model)
        {
            if (roles.Contains(Role.Trainer))
            {
                var trainer = await _context.Trainers.SingleOrDefaultAsync(u => u.UserId == model.User.Id);

                if (trainer == null)
                {
                    trainer = new Models.Trainer()
                    {
                        UserId = model.User.Id,
                        Specialty = model.Specialty
                    };
                    _context.Trainers.Add(trainer);
                }
                else
                    trainer.Specialty = model.Specialty;

                await _context.SaveChangesAsync();
            }

            return model;
        }
    }
}