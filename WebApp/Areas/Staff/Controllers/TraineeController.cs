using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Utils;
using WebApp.ViewModels;
using X.PagedList;

namespace WebApp.Areas.Staff.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class TraineeController : BaseAccountController
    {
        protected override void SetManagedRoles()
        {
            _managedRoles.Add(Role.Trainee);
        }

        protected async Task<IPagedList<ApplicationUser>> GetPagedNames(IQueryable<ApplicationUser> users, int page)
        {
            const int pageSize = 3;

            if (page < 1)
                return null;

            var listPaged = await users.ToPagedListAsync(page, pageSize);

            if (listPaged.PageNumber != 1 && page > listPaged.PageCount)
                return null;

            return listPaged;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string name, int? age, int? page)
        {
            var traineeRole = await RoleManager.FindByNameAsync(Role.Trainee);

            var trainees = _context.Users
                .Include(u => u.Trainee)
                .Where(x => x.Roles
                    .All(r => r.RoleId == traineeRole.Id));

            if (name != null)
            {
                name = name.Trim().ToLower();
                if (name != "")
                    trainees = trainees.Where(u => u.UserName.ToLower().Contains(name));
            }

            if (age != null)
            {
                trainees = trainees.Where(u => u.Age == age);
            }

            trainees = trainees.OrderBy(u => u.Email);

            var listPaged = await GetPagedNames(trainees, page ?? 1);

            if (listPaged == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(listPaged);
        }

        protected override async Task<UserProfileViewModel> GetUserProfile(UserProfileViewModel model)
        {
            var roles = model.Roles;

            if (roles.Contains(Role.Trainee))
            {
                var trainee = await _context.Trainees.SingleOrDefaultAsync(u => u.UserId == model.User.Id);

                if (trainee == null)
                {
                    _ = await AddTrainee(new Models.Trainee() { UserId = model.User.Id });
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

        protected override async Task<int> UpdateUserProfile(UserProfileViewModel model)
        {
            int affectedRow = 0;
            var roles = model.Roles;

            if (roles.Any(r => r == Role.Trainee))
            {
                var trainee = await _context.Trainees.SingleOrDefaultAsync(u => u.UserId == model.User.Id);
                trainee.Education = model.Education;
                trainee.BirthDate = model.BirthDate;
                affectedRow += await _context.SaveChangesAsync();
            }
            return affectedRow;
        }
    }
}