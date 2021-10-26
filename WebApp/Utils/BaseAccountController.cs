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
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace WebApp.Utils
{
    public abstract class BaseAccountController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly List<string> _managedRoles;
        protected ApplicationRoleManager _roleManager;
        protected ApplicationSignInManager _signInManager;
        protected ApplicationUserManager _userManager;

        protected abstract void SetManagedRoles();

        public BaseAccountController()
        {
            _context = new ApplicationDbContext();
            _managedRoles = new List<string>();
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

        protected async Task<IList<string>> GetUserRoles(string userId)
        {
            var roles = await UserManager.GetRolesAsync(userId);

            // check if all roles of user is in managed by controllers
            if (IsUserManagedByRoles(roles))
                return roles;
            else
                return null;
        }

        protected bool IsUserManagedByRoles(IList<string> roles)
        {
            if (roles.Count == 0)
                return false;

            foreach (var role in roles)
                if (!IsUserManagedByRole(role))
                    return false;

            return true;
        }

        protected bool IsUserManagedByRole(string role)
        {
            if (role == null)
                return false;

            foreach (var managedRole in _managedRoles)
            {
                if (role == managedRole)
                    return true;
            }

            return false;
        }

        [HttpGet]
        public ActionResult Create()
        {
            AccountRegisterViewModel model = new AccountRegisterViewModel()
            {
                Roles = _managedRoles
            };
            return View("Account.Register", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (IsUserManagedByRole(model.Role))
                {
                    var user = new ApplicationUser
                    {
                        FullName = model.FullName,
                        Email = model.Email,
                        Age = model.Age,
                        Address = model.Address,
                        UserName = model.Email,
                    };

                    var result = await UserManager.CreateAsync(user, model.Password);

                    // set role and add profile
                    if (result.Succeeded)
                    {
                        if (model.Role == Role.Trainer)
                        {
                            Trainer trainer = new Trainer()
                            {
                                UserId = user.Id,
                                Specialty = model.Specialty,
                            };
                            _ = await AddTrainer(trainer);
                        }
                        else if (model.Role == Role.Trainee)
                        {
                            Trainee trainee = new Trainee()
                            {
                                UserId = user.Id,
                                Education = model.Education,
                                BirthDate = model.BirthDate,
                            };
                            _ = await AddTrainee(trainee);
                        }

                        await UserManager.AddToRoleAsync(user.Id, model.Role);

                        return RedirectToAction("Index");
                    }
                    else
                        AddErrors(result);
                }
                else
                    ModelState.AddModelError("", "Cannot create account with role is" + model.Role);
            }

            model.Roles = _managedRoles;
            return View(model);
        }

        protected async Task<int> AddTrainer(Trainer trainer)
        {
            _context.Trainers.Add(trainer);

            return await _context.SaveChangesAsync();
        }

        protected async Task<int> AddTrainee(Trainee trainee)
        {
            _context.Trainees.Add(trainee);

            return await _context.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            UserViewModel model = await GetUserViewModel(id);

            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id, bool saveChangesError = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            UserViewModel model = await GetUserViewModel(id);
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
                return RedirectToAction("Index");
            }

            IdentityResult result = await UserManager.DeleteAsync(user);

            if (result.Succeeded)
                return RedirectToAction("Index");
            else
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            UserViewModel model = await GetUserViewModel(id);

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

                var roles = await GetUserRoles(user.Id);
                if (roles == null)
                    return HttpNotFound();

                var userinDb = await UserManager.FindByIdAsync(user.Id);

                if (userinDb == null)
                    return HttpNotFound();
                userinDb.FullName = user.FullName;
                userinDb.Age = user.Age;
                userinDb.Address = user.Address;
                userinDb.Email = user.Email;
                userinDb.UserName = user.Email;

                _ = await UpdateUserProfile(model);

                IdentityResult result = await UserManager.UpdateAsync(userinDb);

                if (result.Succeeded)
                    return RedirectToAction("Index");
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

            return View("Account.ResetPassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "The user does not exist.");
                    return View("Account.ResetPassword", model);
                }

                var roles = await UserManager.GetRolesAsync(user.Id);

                if (!IsUserManagedByRoles(roles))
                {
                    ModelState.AddModelError("", "The user cannot be reset. Permission is denied.");
                    return View("Account.ResetPassword", model);
                }

                model.Code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

                if (result.Succeeded)
                {
                    ViewBag.Email = model.Email;
                    return View("Account.ResetPasswordConfirmation");
                }
                else
                    AddErrors(result);
            }

            return View("Account.ResetPassword", model);
        }

        protected async Task<UserViewModel> GetUserViewModel(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var roles = await GetUserRoles(userId);
            if (roles == null)
                return null;

            var model = new UserViewModel()
            {
                User = user,
                Roles = new List<string>(roles)
            };

            model = await GetUserProfile(model);

            return model;
        }

        protected virtual async Task<UserViewModel> GetUserProfile(UserViewModel model)
        {
            var roles = model.Roles;

            if (roles.Contains(Role.Trainer))
            {
                var trainer = await _context.Trainers.SingleOrDefaultAsync(u => u.UserId == model.User.Id);

                if (trainer == null)
                {
                    _ = await AddTrainer(new Trainer() { UserId = model.User.Id });
                    model.Specialty = null;
                }
                else
                {
                    model.Specialty = trainer.Specialty;
                }
            }

            if (roles.Contains(Role.Trainee))
            {
                var trainee = await _context.Trainees.SingleOrDefaultAsync(u => u.UserId == model.User.Id);

                if (trainee == null)
                {
                    _ = await AddTrainee(new Trainee() { UserId = model.User.Id });
                    model.Education = null;
                    model.BirthDate = null;
                }
                else
                {
                    model.Education = trainee.Education;
                    model.BirthDate = trainee.BirthDate;
                }
            }
            return model;
        }

        protected virtual async Task<int> UpdateUserProfile(UserViewModel model)
        {
            int affectedRow = 0;
            if (_managedRoles.Any(r => r == Role.Trainer))
            {
                var trainer = await _context.Trainers.SingleOrDefaultAsync(u => u.UserId == model.User.Id);
                trainer.Specialty = model.Specialty;
                affectedRow += await _context.SaveChangesAsync();
            }

            if (_managedRoles.Any(r => r == Role.Trainee))
            {
                var trainee = await _context.Trainees.SingleOrDefaultAsync(u => u.UserId == model.User.Id);
                trainee.Education = model.Education;
                trainee.BirthDate = model.BirthDate;
                affectedRow += await _context.SaveChangesAsync();
            }
            return affectedRow;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //protected abstract Task<UserViewModel> GetUserProfile(UserViewModel model);
        //protected abstract Task<int> UpdateUserProfile(UserViewModel model);
    }
}