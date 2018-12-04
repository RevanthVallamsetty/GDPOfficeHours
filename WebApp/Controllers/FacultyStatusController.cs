using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.DAL;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class FacultyStatusController : Controller
    {
        private OfficeHoursContext db = new OfficeHoursContext();
        // GET: FacultyStatus
        [HttpGet]
        public ActionResult SelectStatus()
        {
            List<string> flstatl = db.facultyStatuses.Select(s => s.Status).ToList();
            SelectList statusList = new SelectList(flstatl);
            ViewBag.StaList = statusList;
            return View();
        }

        [HttpPost]
        public ActionResult SelectStatus(String Status)
        {
            String stat = Request.Form["statusList"];
            if (Session["facultymail"] != null && stat != "Select a status")
            {
                Faculty fac = db.faculties.Find(Session["facultymail"]);
                if (fac != null)
                {
                    fac.Status = stat;
                    db.SaveChanges();
                }
            }
            else
            {
                return RedirectToAction("SelectStatus").Error("Select any Status from dropdown");
            }
            return RedirectToAction("Schedule","Home");
        }

        [HttpPost]
        public ActionResult CreateNew()
        {
            String newStat = Request.Form["newStatus"];
            if (Session["facultymail"] != null && newStat != "")
            {
                FacultyStatus facultyStatuses = db.facultyStatuses.Find(newStat);
                if (facultyStatuses == null)
                {
                    FacultyStatus newStatus = new FacultyStatus();
                    newStatus.Status = newStat;
                    db.facultyStatuses.Add(newStatus);
                    db.SaveChanges();
                    if (Session["facultymail"] != null)
                    {
                        Faculty fac = db.faculties.Find(Session["facultymail"]);
                        if (fac != null)
                        {
                            fac.Status = newStat;
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("Schedule", "Home");
                }
              
            }
                return RedirectToAction("SelectStatus").Error("Incorrect status Message");
        }
    }
}