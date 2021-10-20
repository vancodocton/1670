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

namespace WebApp.Areas.Trainer.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public TrainerController()
        {
            _context = new ApplicationDbContext();
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

        public async Task<ActionResult> Details(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var profile = await _context.Trainees.SingleOrDefaultAsync(p => p.UserId == user.Id);

            var model = new UserViewModel()
            {
                User = user,
                Roles = new List<string>(await UserManager.GetRolesAsync(user.Id)),
                Education = profile.Education,
                BirthDate = profile.BirthDate
            };

            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var roles = await UserManager.GetRolesAsync(user.Id);
            var model = new UserViewModel()
            {
                User = user,
                Roles = new List<string>(roles)
            };

            Trainee profile = await _context.Trainees.SingleOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                profile = new Trainee()
                {
                    UserId = user.Id,
                    BirthDate = null,
                    Education = null
                };
                _context.Trainees.Add(profile);
                await _context.SaveChangesAsync();
            }
            else
            {
                model.Education = profile.Education;
                model.BirthDate = profile.BirthDate;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = model.User;
                var userinDb = await UserManager.FindByIdAsync(user.Id);

                if (userinDb == null)
                    return HttpNotFound();
                userinDb.FullName = user.FullName;
                userinDb.Age = user.Age;
                userinDb.Address = user.Address;
                userinDb.Email = user.Email;

                IdentityResult result = await UserManager.UpdateAsync(userinDb);

                var profile = await _context.Trainees.SingleOrDefaultAsync(p => p.UserId == user.Id);

                profile.Education = model.Education;
                profile.BirthDate = model.BirthDate;
                await _context.SaveChangesAsync();

                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}