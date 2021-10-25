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
        protected override void SetManagedRoles()
        {
            _managedRoles.Add(Role.Staff);
            _managedRoles.Add(Role.Trainer);
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var model = new List<GroupedUsersViewModel<ApplicationUser>>();

            foreach (var roleName in _managedRoles)
            {
                var role = await RoleManager.FindByNameAsync(roleName);

                var groupedUsers = new GroupedUsersViewModel<ApplicationUser>()
                {
                    Type = roleName,
                    Users = await _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(role.Id)).ToListAsync(),
                };
                model.Add(groupedUsers);
            }

            return View(model);
        }

        protected override async Task<UserViewModel> GetUserProfile(UserViewModel model)
        {
            var roles = model.Roles;

            if (roles.Contains(Role.Trainer))
            {
                var trainer = await _context.Trainers.SingleOrDefaultAsync(u => u.UserId == model.User.Id);

                if (trainer == null)
                {
                    _ = await AddTrainer(new Models.Trainer() { UserId = model.User.Id });
                    model.Specialty = null;
                }
                else
                {
                    model.Specialty = trainer.Specialty;
                }
            }
            return model;
        }

        protected override async Task<int> UpdateUserProfile(UserViewModel model)
        {
            int affectedRow = 0;
            var roles = model.Roles;

            if (roles.Any(r => r == Role.Trainer))
            {
                var trainer = await _context.Trainers.SingleOrDefaultAsync(u => u.UserId == model.User.Id);
                trainer.Specialty = model.Specialty;
                affectedRow += await _context.SaveChangesAsync();
            }

            return affectedRow;
        }

    }
}