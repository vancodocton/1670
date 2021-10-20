using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using WebApp.Models;
using WebApp.Utils;

[assembly: OwinStartupAttribute(typeof(WebApp.Startup))]
namespace WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
        }
        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // In Startup iam creating first Admin Role and creating a default Admin User     
            if (!roleManager.RoleExists(Role.Admin))
            {
                var role = new IdentityRole(Role.Admin);
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "admin1@g.c";
                user.Email = "admin1@g.c";

                string userPWD = "asd@12E";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    UserManager.AddToRole(user.Id, Role.Admin);
                }
            }

            if (!roleManager.RoleExists(Role.Staff))
            {
                var role = new IdentityRole(Role.Staff);
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists(Role.Trainer))
            {
                var role = new IdentityRole(Role.Trainer);
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists(Role.Trainee))
            {
                var role = new IdentityRole(Role.Trainee);
                roleManager.Create(role);
            }
        }
    }
}
