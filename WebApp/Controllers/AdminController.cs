using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Utils;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;
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

        public AdminController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        public ActionResult Index()
        {
            string staffRoleId = _context.Roles.SingleOrDefault(r => r.Name == Role.Staff).Id;
            string trainerRoleId = _context.Roles.SingleOrDefault(r => r.Name == Role.Trainer).Id;

            var model = new GroupedUsersViewModel()
            {
                Staffs = _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(staffRoleId)).ToList(),
                Trainers = _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(trainerRoleId)).ToList(),
                Trainees = null
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult CreateStaffAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateStaffAccount(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, Role.Staff);
                    return RedirectToAction("Index", "Admin");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public ActionResult CreateTrainerAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTrainerAccount(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, Role.Trainer);
                    return RedirectToAction("Index", "Admin");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DetailsAccount(string id)
        {
            //var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
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

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteAccount(string id, bool? saveChangesError = false)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new UserViewModel()
            {
                User = user,
                Roles = new List<string>(await UserManager.GetRolesAsync(user.Id))
            };

            if (saveChangesError == true)
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(model);
        }

        [HttpPost, ActionName("DeleteAccount")]
        public async Task<ActionResult> ConfirmedDeleteAccount(string id)
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

            return RedirectToAction(nameof(DeleteAccount), new { id, saveChangesError = true });
        }

        [HttpGet]
        public async Task<ActionResult> EditAccount(string id)
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

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditAccount(UserViewModel model)
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
            if (!roles.Any(r => r == Role.Staff || r == Role.Trainer))
            {
                ViewBag.ErrorMessage = "The user cannot be reset. Permission is denied.";
                return View(model);
            }

            model.Code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation), user.Email);
            }

            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation(string email)
        {
            return View(email);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}

