using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Areas.Staff.Controllers
{
    public class AssignController : Controller
    {
        // GET: Staff/Assign
        public ActionResult Index()
        {
            return View();
        }

        // GET: Staff/Assign/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Staff/Assign/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Staff/Assign/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Staff/Assign/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Staff/Assign/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Staff/Assign/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Staff/Assign/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
