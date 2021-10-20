using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Areas.Admin.ViewModels;
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

    }
}