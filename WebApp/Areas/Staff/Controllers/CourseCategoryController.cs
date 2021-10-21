using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Utils;

namespace WebApp.Areas.Staff.Controllers
{
    [Authorize(Roles = Role.Staff)]
    public class CourseCategoryController : Controller
    { 
        private ApplicationDbContext _context = new ApplicationDbContext();

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var categories = await _context.CourseCategories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CourseCategory model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _context.CourseCategories.Add(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                ViewBag.Message = e.InnerException;
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CourseCategory model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _context.CourseCategories.Add(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                ViewBag.Message = e.InnerException;
                return View(model);
            }

            return RedirectToAction(nameof(CourseCategory));
        }
    }

}