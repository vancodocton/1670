using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Areas.Trainee.Controllers
{
    public class TraineeController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public ActionResult Index()
        {
            string traineeId = User.Identity.GetUserId();
            var trainee = _context.Users.SingleOrDefault(t => t.Id == traineeId);
            var courses = _context.Courses
                .Include(t => t.CourseCategory)
                .Where(t => t.Trainees.Any(c => c.UserId == traineeId))
                .ToList();
            return View(courses);
        }
        public ActionResult ViewTrainee(int courseId)
        {
            var course = _context.Courses.SingleOrDefault(c => c.Id == courseId);
            var trainees = _context.Trainees
                .Include(t => t.User)
                .Where(t => t.Courses.Any(c => c.Id == courseId))
                .ToList();
            var mine = trainees.Single(t => t.UserId == User.Identity.GetUserId());
            trainees.Remove(mine);
            for(int i = 0; i < trainees.Count; i++)
            {
                if(trainees[i].UserId == User.Identity.GetUserId())
                {
                    trainees.RemoveAt(i);
                    break;
                }
                    
            }

            return View(trainees);
        }
    }
}