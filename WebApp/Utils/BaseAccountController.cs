using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.ViewModels;
using WebApp.Models;

namespace WebApp.Utils
{
    public abstract class BaseAccountController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly List<string> roles;
        protected ApplicationRoleManager _roleManager;
        protected ApplicationSignInManager _signInManager;
        protected ApplicationUserManager _userManager;

        public BaseAccountController()
        {
            _context = new ApplicationDbContext();
            roles = new List<string>();
            SetManagedRoles();
        }

        public BaseAccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager) : this()
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

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

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        protected abstract void SetManagedRoles();

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var model = new List<GroupedUsersViewModel>();

            foreach (var roleName in roles)
            {
                var role = await RoleManager.FindByNameAsync(roleName);

                var groupedUsers = new GroupedUsersViewModel()
                {
                    Type = roleName,
                    Users = await _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(role.Id)).ToListAsync(),
                };
                model.Add(groupedUsers);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            AccountRegisterViewModel model = new AccountRegisterViewModel()
            {
                Roles = roles
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
                    await UserManager.AddToRoleAsync(user.Id, model.Role);

                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
            }

            model.Roles = roles;
            return View(model);
        }

        protected abstract Task<UserViewModel> LoadUserViewModel(string userId);
        protected abstract Task<UserViewModel> LoadUserProfile(UserViewModel model);
        protected abstract Task<UserViewModel> UpdateUserProfile(UserViewModel model);

        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            UserViewModel model = await LoadUserViewModel(id);

            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id, bool saveChangesError = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            UserViewModel model = await LoadUserViewModel(id);
            if (model == null)
                return HttpNotFound();

            if (saveChangesError)
            {
                ModelState.AddModelError("",
                    "Delete failed.Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            UserViewModel model = await LoadUserViewModel(id);

            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                userinDb.UserName = user.Email;

                IdentityResult result = await UserManager.UpdateAsync(userinDb);
                model = await UpdateUserProfile(model);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(result);
            }

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

            if (!roles.All(r => this.roles.Contains(r)))
            {
                ViewBag.ErrorMessage = "The user cannot be reset. Permission is denied.";
                return View(model);
            }

            model.Code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation), null, new { email = user.Email });
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}