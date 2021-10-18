using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApp.Areas.Admin.ViewModels;
using WebApp.Models;
using WebApp.Utils;
using WebApp.ViewModels;
using PagedList;

namespace WebApp.Areas.Staff.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class TraineeController : Controller
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
        public TraineeController()
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
        [HttpGet]
        public ActionResult Create()
        {
            AccountRegisterViewModel model = new AccountRegisterViewModel()
            {
                Roles = new List<string> { Role.Trainee }
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, Role.Trainee);
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            model.Roles = new List<string> { Role.Trainee };
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
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

        [HttpGet]
        public async Task<ActionResult> Delete(string id, bool? saveChangesError = false)
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

            if (saveChangesError == true)
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> ConfirmedDelete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            IdentityResult result = await UserManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
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
                    UserId = user.Id ,
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

        [HttpGet]
        public ActionResult ResetPassword(string email = null)
        {
            if (email == null)
                return View();

            var model = new ResetPasswordViewModel()
            {
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ViewBag.ErrorMessage = "The user does not exist";
                return View(model);
            }

            var roles = await UserManager.GetRolesAsync(user.Id);
            if (!roles.All(r => r == Role.Trainee))
            {
                ViewBag.ErrorMessage = "The user cannot be reset. Permission is denied.";
                return View(model);
            }

            model.Code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Trainee", new { email = user.Email });
            }

            AddErrors(result);
            return View();
        }

        [Route("{email}")]
        [HttpGet]
        public ActionResult ResetPasswordConfirmation(string email)
        {
            ViewBag.Email = email;
            return View();
        }

    }
}