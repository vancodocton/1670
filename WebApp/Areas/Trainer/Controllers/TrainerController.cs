﻿using Microsoft.AspNet.Identity;
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
            var trainer = await _context.Trainers                
                .SingleOrDefaultAsync(t => t.UserId == traierId);
            var courses = _context.Courses
                .Include(c => c.CourseCategory)
                .Where(c => c.Trainers.Any(t => t.UserId == traierId))
                .ToList();
            return View(courses);
        }

        public async Task<ActionResult> ViewTrainee(int courseId)
        {            
            var course = await _context.Courses.SingleOrDefaultAsync(t => t.Id == courseId);

            var trainees =  _context.Trainees
                .Include(t => t.User)
                .Where(t => t.Courses.Any(c => c.Id == course.Id))
                .ToList();

            return View(trainees);
            
        }
    }
}