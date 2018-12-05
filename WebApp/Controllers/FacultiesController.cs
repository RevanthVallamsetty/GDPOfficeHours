using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class FacultiesController : Controller
    {
        private OfficeHoursContext db = new OfficeHoursContext();

        
        // GET: Faculties/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Faculties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Email,Id,first_Name,last_Name")] Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                if(db.faculties.Find(faculty.Email) != null)
                {
                    return RedirectToAction("Index", "Home").Error("Faculty already present.");
                }

                db.faculties.Add(faculty);
                db.SaveChanges();
                return RedirectToAction("Index", "Events").Success("Faculty Created");
            }
            return RedirectToAction("Index", "Events").Success("Faculty Created");
        }
        
    }
}
