using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.ViewModels;
using X.PagedList;

namespace WebApp.Areas.Trainer.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        
        public async Task<ActionResult> Index()
        {
            string traierId = User.Identity.GetUserId();
            var trainer = await _context.Trainers.SingleOrDefaultAsync(t => t.UserId == traierId);
            var courses = trainer.Courses.ToList();
            return View(courses);
        }

        public async Task<ActionResult> ViewTrainee(int courseId)
        {
            string myUserId = User.Identity.GetUserId();

            var trainer = await _context.Trainers.SingleAsync(t => t.UserId == myUserId);

            var courses = trainer.Courses.ToListAsync();

            var coursetrainees = trainer.Courses
                .Select(c => new GroupedUsersViewModel<Trainee>
                {
                    Type = c.Name,
                    Users = c.Trainees.ToList(),
                })
                .ToList();

            string userId = User.Identity.GetUserId();
            var myCourse = await _context.Courses.SingleOrDefaultAsync(c => c.Id == courseId);

            if (myCourse.Trainers.Any(t => t.UserId == myUserId))
            {
                var myTrainees = myCourse.Trainees.ToListAsync();
                return View(myTrainees);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }
    }
}