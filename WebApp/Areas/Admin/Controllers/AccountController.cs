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